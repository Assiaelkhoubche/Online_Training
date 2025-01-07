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
using System;
using Online_training.Server.Services;
using Online_training.Server.Helpers;

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

////////////////
///






builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
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

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();


app.UseCors("AllowFrontend");

app.UseSession();

app.UseDefaultFiles();
app.UseStaticFiles();



//app.MapPost("/register", (RegisterModel model) =>
//{

//});



//api for user info manpulation

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
        await signInManager.SignInAsync(user, isPersistent: true);
        return Results.Ok(new { message = "You are successfully signed in" });
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


// api formation manipulation
app.MapPost("/add-category", async (ApplicationDbContext dbContext, HttpRequest request) =>
{

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

        var categoryName = formData["name"];

        var newCategory = new Category { Name = categoryName };
        dbContext.Categories.Add(newCategory);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0)
        {
            return Results.BadRequest(new { message = "failed to add this category" });
        }
        return Results.Ok(new { message = "success", newCategory });


});






app.MapPost("/add-formation", async (ApplicationDbContext dbContext, HttpRequest request) =>
{
    var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
    Console.WriteLine($"Raw Request Body: {requestBody}");

    // Deserialize as JsonDocument for safe parsing
    using var jsonDoc = JsonDocument.Parse(requestBody);
    var root = jsonDoc.RootElement;

    // Parse individual properties
    var title = root.GetProperty("title").GetString();
    var description = root.GetProperty("description").GetString();
    var categoryName = root.GetProperty("categoryName").GetString();
    var trainerId = root.GetProperty("trainerId").GetString();
    var price = root.GetProperty("price").GetDecimal();
    var level = root.GetProperty("level").GetString();
    var language = root.GetProperty("language").GetString();
    var ImageFormation = root.GetProperty("ImageFormation").GetString();
    // Parse sections (if available)
    var sections = root.TryGetProperty("sections", out var sectionsElement) && sectionsElement.ValueKind == JsonValueKind.Array
        ? sectionsElement.EnumerateArray().ToList()
        : new List<JsonElement>();

    // Query Category
    var categoryId = await dbContext.Categories
        .Where(ct => ct.Name == categoryName)
        .Select(ct => ct.Id)
        .FirstOrDefaultAsync();

    if (categoryId == 0)
    {
        return Results.BadRequest(new { message = "No category found in the database", categoryName, title });
    }

    // Find Trainer
    var trainer = await dbContext.Trainers.FindAsync(trainerId);
    if (trainer == null)
    {
        return Results.BadRequest(new { message = "No Trainer found" });
    }

    // Create Formation
    var formation = new Formation
    {
        Title = title,
        Description = description,
        CategoryId = categoryId,
        TrainerId = trainerId,
        Level = level,
        Language = language,
        Price = price,
        sutudent = 12, // Default value
        ImageFormation= ImageFormation
    };

    dbContext.Formations.Add(formation);
    await dbContext.SaveChangesAsync();

    // Add Sections and Videos
    foreach (var sectionElement in sections)
    {
        var sectionTitle = sectionElement.GetProperty("title").GetString();
        var sectionOrderIndex = sectionElement.GetProperty("orderIndex").GetInt32();
        var isPreview = sectionElement.GetProperty("isPreview").GetBoolean();

        var newSection = new Section
        {
            Title = sectionTitle,
            FormationId = formation.Id,
            OrderIndex = sectionOrderIndex,
            IsPreview = isPreview
        };

        dbContext.Sections.Add(newSection);
        await dbContext.SaveChangesAsync();

        if (sectionElement.TryGetProperty("videos", out var videosElement) && videosElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var videoElement in videosElement.EnumerateArray())
            {
                var video = new Video
                {
                    Link = videoElement.GetProperty("link").GetString(),
                    ThumbnailLink = videoElement.GetProperty("thumbnailLink").GetString(),
                    Duration = TimeSpan.TryParse(videoElement.GetProperty("duration").GetString(), out var duration) ? duration : TimeSpan.FromMinutes(5),
                    SectionId = newSection.Id,
                };

                dbContext.Videos.Add(video);
                await dbContext.SaveChangesAsync(); // Save video immediately
            }
        }
    }

    return Results.Ok(new { formation });
});


app.MapGet("/get-formations", async (ApplicationDbContext dbContext) =>
{
    var formations = await dbContext.Formations
                                    .Include(f => f.Sections)           // Include Sections for each Formation
                                    .ThenInclude(s => s.Videos)         // Include Videos for each Section
                                    .ToListAsync();

    if (formations == null || !formations.Any())
    {
        return Results.NotFound("No formations found.");
    }

    // Project the data to return in the desired format
    var result = formations.Select(f => new
    {
        f.Id,
        f.Title,
        f.ImageFormation,
        f.Description,
        f.Level,
        f.Language,
        f.sutudent,
        f.Price,
        f.oldPrice,
        f.CategoryId,
        Category = f.Category != null ? new { f.Category.Id, f.Category.Name } : null,  // Assuming Category has an Id and Name
        f.TrainerId,
        Sections = f.Sections.Select(s => new
        {
            s.Id,
            s.OrderIndex,
            s.IsPreview,
            s.Title,
            Videos = s.Videos.Select(v => new
            {
                v.Id,
                v.Link,
                v.ThumbnailLink,
                v.Duration
            })
        })
    }).ToList();

    return Results.Ok(result);
});

app.MapGet("/get-formation/{id}", async (int id, ApplicationDbContext dbContext) =>
{
    var formation = await dbContext.Formations
                                   .Include(f => f.Sections)           // Include Sections for the Formation
                                   .ThenInclude(s => s.Videos)         // Include Videos for each Section
                                   .FirstOrDefaultAsync(f => f.Id == id); // Find formation by ID

    if (formation == null)
    {
        return Results.NotFound($"Formation with ID {id} not found.");
    }

    // Project the data to return in the desired format
    var result = new
    {
        formation.Id,
        formation.Title,
        formation.ImageFormation,
        formation.Description,
        formation.Level,
        formation.Language,
        formation.sutudent,
        formation.Price,
        formation.oldPrice,
        formation.CategoryId,
        Category = formation.Category != null ? new { formation.Category.Id, formation.Category.Name } : null,  // Assuming Category has an Id and Name
        formation.TrainerId,
        Sections = formation.Sections.Select(s => new
        {
            s.Id,
            s.OrderIndex,
            s.IsPreview,
            s.Title,
            Videos = s.Videos.Select(v => new
            {
                v.Id,
                v.Link,
                v.ThumbnailLink,
                v.Duration
            })
        })
    };

    return Results.Ok(result);
});

app.MapGet("/get-trainer/{id}", async (string id, ApplicationDbContext dbContext) =>
{
    // Find the trainer by ID
    var trainer = await dbContext.Trainers
                                  .Where(t => t.Id == id)
                                  .Select(t => new
                                  {
                                      t.PictureUrl,  // Assuming PictureUrl is a property of Trainer
                                      t.UserName     // Assuming UserName is a property of Trainer
                                  })
                                  .FirstOrDefaultAsync();

    if (trainer == null)
    {
        return Results.NotFound("Trainer not found.");
    }

    return Results.Ok(trainer);
});




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
