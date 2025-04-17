// AuthService.cs
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(UserRegistrationDto registration);
    Task<string> GenerateJwtTokenAsync(User user);
    Task<UserDto> GetUserByIdAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context, 
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Utilisateur non trouvé" };
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result != PasswordVerificationResult.Success)
        {
            return new AuthResult { Success = false, ErrorMessage = "Mot de passe incorrect" };
        }

        if (!user.IsActive)
        {
            return new AuthResult { Success = false, ErrorMessage = "Compte désactivé" };
        }

        var token = await GenerateJwtTokenAsync(user);
        return new AuthResult { Success = true, Token = token, UserId = user.Id };
    }

    public async Task<AuthResult> RegisterAsync(UserRegistrationDto registration)
    {
        // Vérification si l'email existe déjà
        if (await _context.Users.AnyAsync(u => u.Email == registration.Email))
        {
            return new AuthResult { Success = false, ErrorMessage = "Cet email est déjà utilisé" };
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registration.Email,
            FirstName = registration.FirstName,
            LastName = registration.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            OrganizationId = registration.OrganizationId
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registration.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Ajout du rôle par défaut
        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == registration.DefaultRole);
        if (defaultRole != null)
        {
            _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = defaultRole.Id });
            await _context.SaveChangesAsync();
        }

        var token = await GenerateJwtTokenAsync(user);
        return new AuthResult { Success = true, Token = token, UserId = user.Id };
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var userRoles = await _context.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("org", user.OrganizationId.ToString())
        };

        // Ajouter les rôles
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            
            // Ajouter les permissions
            foreach (var rolePermission in userRole.Role.RolePermissions)
            {
                claims.Add(new Claim("permission", rolePermission.Permission.Name));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddHours(1);

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            OrganizationId = user.OrganizationId,
            OrganizationName = user.Organization?.Name,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public string ErrorMessage { get; set; }
}

public class UserRegistrationDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid OrganizationId { get; set; }
    public string DefaultRole { get; set; } = "OrganizationAdmin";
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public List<string> Roles { get; set; }
}