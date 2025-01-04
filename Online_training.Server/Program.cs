using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Online_training.Server.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthorization();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout duration
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowCredentials() //to allow cookie
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }); 
});




builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


app.UseCors("AllowFrontend");

app.UseSession();

app.UseDefaultFiles();
app.UseStaticFiles();



//app.MapPost("/register", (RegisterModel model) =>
//{

//});

app.MapPost("/register", async (UserManager<User> userManager, HttpRequest request) =>
{
    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);


    var roleUser = formData["role"];
    var email=formData["email"];
    var userName=formData["userName"];
    var password = formData["password"];
    var confirmPassword = formData["confirmPassword"];

    if (password != confirmPassword)
    {
        return Results.BadRequest(new { message = "Passwords do not match." });
    }

    User user;

    if (roleUser == "Trainer")
    {
        var speciality = formData["speciality"];
         user = new Trainer
        {
            Email = email,
            UserName = userName,
            Speciality = speciality,
           
        };
    }else if (roleUser=="Participant")
    {
        user = new Participant
        {
            Email = email,
            UserName = userName,
            IsPro = false
        };
    }
    else
    {
        return Results.BadRequest(new { message = "Failed to register the user" });
    }

    var result= await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        
        if(roleUser == "Participant")
        {
            var  role = await userManager.AddToRoleAsync(user, "User");
            if (role.Succeeded)
            {
                return Results.Ok(new { message = "User registered successfully and assigned the User role." });
            }
            else
            {
                return Results.BadRequest(new { message = "Failed to assign role to user." });
            }
        }else if(roleUser == "Trainer")
        {
            var role = await userManager.AddToRoleAsync(user, "Trainer");
            if (role.Succeeded)
            {
                return Results.Ok(new { message = "Trainer registered successfully and assigned the Trainer role." });
            }
            else
            {
                return Results.BadRequest(new { message = "Failed to assign role to Trainer." });
            }
        }
        else
        {
            return Results.BadRequest(new {message="No role asgned"});
        }
       
        
    }
    else
    {
        return Results.BadRequest(new { message = "Failed to creat a user" });
    }

});


app.MapPost("/login", async (SignInManager<User> signInManager, UserManager<User> userManager, HttpRequest request, ApplicationDbContext dbContext) =>
{
    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

    var email = formData["email"];
    var password = formData["password"];

    var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        return Results.BadRequest(new { message = "Invalid email or password" });
    }

    if (await userManager.IsLockedOutAsync(user))
    {
        return Results.BadRequest(new { message = "Account is locked. Try again later." });
    }

    var Result = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

    if (Result.Succeeded)
    {
        
        

        

        return Results.Ok(new { message = "You are successfully signed in"});
    }
    else
    {
        return Results.BadRequest(new { message = "Invalid email or password" });
    }
});

app.MapGet("/my-info", async(HttpContext httpContext, UserManager<User> userManager) =>
{
    if (httpContext.User.Identity?.IsAuthenticated == true)
    {
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var user= await userManager.FindByIdAsync(userId);


        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(new
            {
                Id = user.Id,
                name = user.UserName,
                email = user.Email,
                bio = user.Bio,
                pictureUrl = user.PictureUrl,
                roles = roles
            });
        }
        else
        {
            return Results.BadRequest(new {message="probleme with finding the user"});
        }

       
    }
    return Results.Unauthorized();
}).RequireAuthorization();

app.MapPost("/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();

}).RequireAuthorization();


var group = app.MapGroup("/").DisableAntiforgery();

group.MapPost("/upload-image", [IgnoreAntiforgeryToken] async ( IFormFile file) =>
{
    if(file==null && file.Length == 0)
    {
        return Results.BadRequest(new { message = "no file uploaded" });
    }
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
   
    //save the file in the server
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var fileUrl = $"http://localhost:5284/uploads/{file.FileName}";
    return Results.Ok(new { imageUrl = fileUrl });

}).AllowAnonymous();

app.MapPost("/edite-profile",async (UserManager<User> userManager, HttpRequest request, SignInManager<User> signInManager, ClaimsPrincipal userClaims)=>
{
    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

    var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var userName = formData["userName"];
    var bio= formData["bio"];
    var pictureUrl= formData["pictureUrl"];

    var user= await userManager.FindByIdAsync(userId);

    if(user == null)
    {
        return Results.BadRequest(new {message="no user found"});
    }

    user.UserName= userName;
    user.Bio= bio;
    user.PictureUrl= pictureUrl;

    var result = await userManager.UpdateAsync(user);

    if (result.Succeeded)
    {
        
        return Results.Ok(new
        {
            message = "your update is succeed",user
            
        });
    }
    else
    {
        return Results.BadRequest(new
        {
            message = "Failed to update profile"
        });
    }

}).RequireAuthorization();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
