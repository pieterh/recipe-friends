using Microsoft.EntityFrameworkCore;
using RecipeFriends.Shared.Data;
using RecipeFriends.Shared.Data.Models;
using RecipeFriends.Shared.DTO;
using SixLabors.ImageSharp.Advanced;
using Image = RecipeFriends.Shared.Data.Models.Image;

namespace RecipeFriends.Services;

public class RecipeService(RecipeFriendsDbContext context) : IRecipeService
{
    private readonly RecipeFriendsDbContext _context = context;

    public async Task<RecipeDetails> GetRecipeDetailsAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var recipe = await _context.Recipes.FindAsync([id], cancellationToken: cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(id));
        return MapToRecipeDetailsDTO(recipe);
    }

    public async Task<ImageData> GetImageDataAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var image = await _context.Images.FindAsync([id], cancellationToken: cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(id));
        return MapToImageDataDTO(image, 200, 200);
    }


    public async Task<ImageData> GetImageDataAsync(
        int id,
        int width,
        int height,
        CancellationToken cancellationToken
    )
    {
        var image = await _context.Images.FindAsync([id], cancellationToken: cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(id));
        return MapToImageDataDTO(image, width, height);
    }

    public async Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken)
    {
        var l = await _context.Recipes.Include(r => r.Tags).ToListAsync(cancellationToken: cancellationToken);
        var r = l.Select(MapToRecipeInfoDTO).ToArray();
        return r;
    }

    public async Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDTO, IEnumerable<ImageData> images, CancellationToken cancellationToken)
    {
        try
        {
            Recipe recipe;
            if (recipeDTO.Id > 0)
            {
                recipe = await UpdateRecipeAsync(recipeDTO, cancellationToken);
                if (recipe == null)
                    return false;

                // only need to remove the inactive images from existing images
                foreach (var delImage in images.Where(x => x.Status == ImageStatus.Inactive))
                {
                    var image = _context.Images.FirstOrDefault((x) => x.Id == delImage.Id);
                    if (image != null)
                    {
                        recipe.Images.Remove(image);
                        _context.Images.Remove(image);
                    }
                }
            }
            else
            {
                recipe = await InsertNewRecipe(recipeDTO);
                if (recipe == null)
                    return false;
            }

            foreach (var img in images.Where(x => x.Status != ImageStatus.Inactive))
            {
                _ = await SaveImageDataAsync(recipe, img);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<Recipe> UpdateRecipeAsync(
        RecipeDetails recipeDTO,
        CancellationToken cancellationToken
    )
    {
        var existingRecipe = await _context.Recipes
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == recipeDTO.Id, cancellationToken);

        if (existingRecipe == null)
            throw new ArgumentOutOfRangeException(nameof(recipeDTO));

        RecipeService.MapRecipeDTOToRecipe(recipeDTO, existingRecipe);

        UpdateRecipeTags(recipeDTO, existingRecipe);
        UpdateRecipeEquipment(recipeDTO, existingRecipe);
        RecipeService.UpdateRecipeIngredients(recipeDTO, existingRecipe);
        //UpdateRecipeImages(recipeDTO, existingRecipe);
        return existingRecipe;
    }

    private async Task<Recipe> InsertNewRecipe(RecipeDetails recipeDTO)
    {
        var newRecipe = new Recipe()
        {
            Title = recipeDTO.Title,
            CategoryId = recipeDTO.Category.Id,
            ShortDescription = recipeDTO.ShortDescription,
            Description = recipeDTO.Description,
            Directions = recipeDTO.Directions,
            PreparationTime = recipeDTO.PreparationTime,
            CookingTime = recipeDTO.CookingTime
        };
        await _context.Recipes.AddAsync(newRecipe);
        UpdateRecipeTags(recipeDTO, newRecipe);
        UpdateRecipeEquipment(recipeDTO, newRecipe);
        RecipeService.UpdateRecipeIngredients(recipeDTO, newRecipe);
        return newRecipe;
    }

    private void UpdateRecipeTags(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        // Handle tags
        // Identify tags that are no longer associated
        var tagsToRemove = existingRecipe.Tags
            .Where(rt => !recipeDTO.Tags.Any(t => t.Id == rt.Id))
            .ToList();

        foreach (var tagToRemove in tagsToRemove)
        {
            existingRecipe.Tags.Remove(tagToRemove);
        }

        // Add new tags
        foreach (var tagDTO in recipeDTO.Tags)
        {
            // Check if the tag already exists in the database
            if (!existingRecipe.Tags.Any(rt => rt.Id == tagDTO.Id))
            {
                var tagRecipe = _context.Tags.FirstOrDefault((x) => x.Id == tagDTO.Id);
                if (tagRecipe == null)
                {
                    tagRecipe = new Tag() { Name = tagDTO.Name };
                    _context.Tags.Add(tagRecipe);
                }
                existingRecipe.Tags.Add(tagRecipe);
            }
        }
    }

    private void UpdateRecipeEquipment(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        // Handle equipment
        // Identify equipment that are no longer associated
        var itemsToRemove = existingRecipe.Equipment
            .Where(rt => !recipeDTO.Tags.Any(t => t.Id == rt.Id))
            .ToList();

        foreach (var equipmentToRemove in itemsToRemove)
        {
            existingRecipe.Equipment.Remove(equipmentToRemove);
        }

        // Add new tags
        foreach (var equipmentDTO in recipeDTO.Equipment)
        {
            // Check if the Equipment already exists in the database
            if (!existingRecipe.Equipment.Any(rt => rt.Id == equipmentDTO.Id))
            {
                var equipmentRecipe = _context.Equipment.FirstOrDefault((x) => x.Id == equipmentDTO.Id);
                if (equipmentRecipe == null)
                {
                    equipmentRecipe = new Equipment() { Name = equipmentDTO.Name };
                    _context.Equipment.Add(equipmentRecipe);
                }
                existingRecipe.Equipment.Add(equipmentRecipe);
            }
        }
    }

    private static void UpdateRecipeIngredients(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        // Handle ingerdients
        // Identify ingredients that are no longer associated
        var ingredientsToRemove = existingRecipe.Ingredients
            .Where(rt => !recipeDTO.Ingredients.Any(t => t.Id == rt.Id))
            .ToList();
        foreach (var ingredientToRemove in ingredientsToRemove)
        {
            existingRecipe.Ingredients.Remove(ingredientToRemove);
        }
        // add new ingredients or update existing
        foreach (var ingredientDTO in recipeDTO.Ingredients)
        {
            var ingredient = existingRecipe.Ingredients.FirstOrDefault(
                i => i.Id == ingredientDTO.Id
            );
            if (ingredient == null)
            {
                // add new ingredient
                ingredient = new Ingredient()
                {
                    Order = ingredientDTO.Order,
                    Name = ingredientDTO.Name,
                    Amount = ingredientDTO.Amount,
                    MeasurementId = ingredientDTO.Measurement.Id
                };
                existingRecipe.Ingredients.Add(ingredient);
            }
            else
            {
                // update existing
                ingredient.Order = ingredientDTO.Order;
                ingredient.Name = ingredientDTO.Name;
                ingredient.Amount = ingredientDTO.Amount;
                ingredient.MeasurementId = ingredientDTO.Measurement.Id;
            }
        }
    }

    private static void UpdateRecipeImages(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        foreach (var image in recipeDTO.Images)
        {
            var imgToUpdate = existingRecipe.Images.First(x => x.Id == image.Id);
            imgToUpdate.Order = image.Order;
            imgToUpdate.Title = image.Title;
            imgToUpdate.Name = image.Name;
        }
    }

    public async Task<bool> SaveImageDataAsync(int recipeId, ImageData img, CancellationToken cancellationToken)
    {
        var recipe = await _context.Recipes.FindAsync([recipeId], cancellationToken: cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(recipeId));
        var retval = await SaveImageDataAsync(recipe, img);
        if (retval)
            await _context.SaveChangesAsync(cancellationToken);
        return retval;
    }

    internal async Task<bool> SaveImageDataAsync(Recipe recipe, ImageData img)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        ArgumentNullException.ThrowIfNull(img);

        if (img.Id > 0)
        {
            var image = await _context.Images.FindAsync([img.Id]) ?? throw new ArgumentOutOfRangeException(nameof(img));
            image.Order = img.Order;
            image.Title = img.Title;
            image.Name = img.Name;
            // not updating the actual data of the image self
            _context.Images.Update(image);

        }
        else
            recipe.Images.Add(RecipeService.MapImageDataDTOToImage(img));

        return true;
    }

    public async Task<TagInfo[]> GetTagsAsync(CancellationToken cancellationToken)
    {
        var tags = await _context.Tags.ToListAsync(cancellationToken: cancellationToken);
        return tags.Select(MapToTagDTO).ToArray();
    }

    public async Task<EquipmentInfo[]> GetEquipmentAsync(CancellationToken cancellationToken)
    {
        var equipment = await _context.Equipment.ToListAsync(cancellationToken: cancellationToken);
        return equipment.Select(MapToEquipmentDTO).ToArray();
    }

    public async Task<CategoryInfo[]> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.Catagories
            .Where(c => c.Status == EntityStatus.Active)
            .ToListAsync(cancellationToken: cancellationToken);
        return categories.Select(MapToCategoryDTO).ToArray();
    }

    public async Task<MeasurementInfo[]> GetMeasurementsAsync(CancellationToken cancellationToken)
    {
        var measurements = await _context.Measurements.ToListAsync(
            cancellationToken: cancellationToken
        );
        return measurements.Select(MapToMeasurementDTO).ToArray();
    }

    #region Map Model To DTO
    private RecipeDetails MapToRecipeDetailsDTO(Recipe recipe)
    {
        // make sure the related lists and references are loaded
        _context.Entry(recipe).Collection(r => r.Ingredients).Load();
        _context.Entry(recipe).Collection(r => r.Tags).Load();
        _context.Entry(recipe).Collection(r => r.Equipment).Load();
        _context.Entry(recipe).Collection(r => r.Images).Load();
        _context.Entry(recipe).Reference(r => r.Category).Load();

        return new RecipeDetails
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Category = MapToCategoryDTO(recipe.Category),
            ShortDescription = recipe.ShortDescription,
            Description = recipe.Description,
            Directions = recipe.Directions,
            PreparationTime = recipe.PreparationTime,
            CookingTime = recipe.CookingTime,
            Ingredients = recipe.Ingredients.Select(MapToIngredientDTO).ToList(),
            Tags = recipe.Tags.Select(rt => new TagInfo() { Id = rt.Id, Name = rt.Name }).ToList(),
            Equipment = recipe.Equipment.Select(rt => new EquipmentInfo() { Id = rt.Id, Name = rt.Name }).ToList(),
            Images = recipe.Images
                .Select(
                    rt => MapToImageInfoDTO(rt)
                )
                .ToList()
        };
    }

    private RecipeInfo MapToRecipeInfoDTO(Recipe recipe)
    {
        // make sure the related lists and references are loaded
        _context.Entry(recipe).Collection(r => r.Tags).Load();
        _context.Entry(recipe).Reference(r => r.Category).Load();
        return new RecipeInfo
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Category = new CategoryInfo()
            {
                Id = recipe.Category.CategoryId,
                Name = recipe.Category.Name
            },
            ShortDescription = recipe.ShortDescription,
            Tags = recipe.Tags.Select(rt => rt.Name).ToList()
        };
    }

    private IngredientDetails MapToIngredientDTO(Ingredient ingredient)
    {
        _context.Entry(ingredient).Reference(r => r.MeasurementNew).Load();
        return new IngredientDetails
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            Measurement = MapToMeasurementDTO(ingredient.MeasurementNew),
            Order = ingredient.Order
        };
    }

    private static Shared.DTO.ImageInfo MapToImageInfoDTO(Image image)
    {
        return new Shared.DTO.ImageInfo()
        {
            Id = image.Id,
            Order = image.Order,
            Title = image.Title,
            Name = image.Name
        };
    }

            // int targetWidth = maxWidth;
        // int targetHeight = maxHeight;

        // // If the image is smaller than the target dimensions, keep its original size.
        // if (slImage.Width < targetWidth && slImage.Height < targetHeight)
        // {
        //     targetWidth = slImage.Width;
        //     targetHeight = slImage.Height;
        // }

        // slImage.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
        // {
        //     Size = new SixLabors.ImageSharp.Size(200, 200),
        //     Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max
        //     // Constrains the resized image to fit the bounds of its container maintaining the original aspect ratio.
        // }));

    private static ImageData MapToImageDataDTO(Image image, int maxWidth, int maxHeight)
    {
        var (data, filename) = HandleImageData(image.Data, image.Name, maxWidth, maxHeight);

        return new ImageData()
        {
            Id = image.Id,
            Order = image.Order,
            Name = filename,
            Title = image.Title,
            Data = data,
            Status = ImageStatus.Active
        };
    }

    private static (byte[], string) HandleImageData(byte[] imageData, string imageName, int maxWidth, int maxHeight)
    {
        using var slImage = SixLabors.ImageSharp.Image.Load(imageData);

        // Resize and crop the image
        ResizeAndCropImage3(slImage, maxWidth, maxHeight);
        var decodedImageFormatName = slImage.Metadata.DecodedImageFormat.Name.ToLower();

        // Save the resized and cropped image as PNG
        return ConvertToPngAndGetData(slImage, imageName);
    }

    private static void ResizeAndCropImage1(SixLabors.ImageSharp.Image slImage, int maxWidth, int maxHeight)
    {
        var originalWidth = slImage.Width;
        var originalHeight = slImage.Height;

        var targetAspectRatio = (float)maxWidth / maxHeight;
        var imageAspectRatio = (float)originalWidth / originalHeight;

        int newWidth, newHeight;

        if (imageAspectRatio > targetAspectRatio)
        {
            // The image is wider than the target dimensions. Resize based on height.
            newHeight = maxHeight;
            newWidth = (int)(newHeight * imageAspectRatio);
        }
        else
        {
            // The image is taller or equal to the target dimensions. Resize based on width.
            newWidth = maxWidth;
            newHeight = (int)(newWidth / imageAspectRatio);
        }

        slImage.Mutate(x => x.Resize(newWidth, newHeight));

        // Crop the image to the desired dimensions from the center.
        var offsetX = (newWidth - maxWidth) / 2;
        var offsetY = (newHeight - maxHeight) / 2;
        var rect = new SixLabors.ImageSharp.Rectangle(offsetX, offsetY, maxWidth, maxHeight);

        slImage.Mutate(x => x.Crop(rect));
    }

    private static void ResizeAndCropImage2(SixLabors.ImageSharp.Image slImage, int maxWidth, int maxHeight)
    {
        var originalWidth = slImage.Width;
        var originalHeight = slImage.Height;

        var targetAspectRatio = (float)maxWidth / maxHeight;
        var imageAspectRatio = (float)originalWidth / originalHeight;

        int cropWidth, cropHeight;

        if (imageAspectRatio > targetAspectRatio)
        {
            // The image is wider than the target dimensions. Crop based on width.
            cropWidth = (int)(originalHeight * targetAspectRatio);
            cropHeight = originalHeight;
        }
        else
        {
            // The image is taller or equal to the target dimensions. Crop based on height.
            cropWidth = originalWidth;
            cropHeight = (int)(originalWidth / targetAspectRatio);
        }

        // Crop the image to the desired aspect ratio from the center.
        var offsetX = (originalWidth - cropWidth) / 2;
        var offsetY = (originalHeight - cropHeight) / 2;
        var rect = new SixLabors.ImageSharp.Rectangle(offsetX, offsetY, cropWidth, cropHeight);

        slImage.Mutate(x => x.Crop(rect));

        // Resize the cropped image to the target dimensions.
        slImage.Mutate(x => x.Resize(maxWidth, maxHeight));
    }

    private static void ResizeAndCropImage3(SixLabors.ImageSharp.Image slImage, int maxWidth, int maxHeight)
    {
        var originalWidth = slImage.Width;
        var originalHeight = slImage.Height;

        // Calculate the new height after resizing to fit maxWidth
        var newHeight = (int)((float)originalHeight / originalWidth * maxWidth);

        // Resize the image to fit the maxWidth
        slImage.Mutate(x => x.Resize(maxWidth, newHeight));

        if (newHeight > maxHeight){
            // Crop the resized image from the center to achieve the desired height
            var offsetY = (newHeight - maxHeight) / 2;
            var rect = new SixLabors.ImageSharp.Rectangle(0, offsetY, maxWidth, maxHeight);

            slImage.Mutate(x => x.Crop(rect));
        }
    }


    private static (byte[], string) ConvertToPngAndGetData(byte[] imageData, string imageName)
    {
        using var slImage = SixLabors.ImageSharp.Image.Load(imageData);
        return ConvertToPngAndGetData(slImage, imageName);
    }

    private static (byte[], string) ConvertToPngAndGetData(SixLabors.ImageSharp.Image slImage, string imageName)
    {
        using var stream = new MemoryStream();
        slImage.SaveAsPng(stream);
        var newName = Path.ChangeExtension(imageName, "png");
        return (stream.ToArray(), newName);
    }


    private TagInfo MapToTagDTO(Tag tag)
    {
        return new TagInfo { Id = tag.Id, Name = tag.Name };
    }

    private EquipmentInfo MapToEquipmentDTO(Equipment equipment)
    {
        return new EquipmentInfo { Id = equipment.Id, Name = equipment.Name };
    }

    private CategoryInfo MapToCategoryDTO(Category category)
    {
        return new CategoryInfo { Id = category.CategoryId, Name = category.Name };
    }

    private MeasurementInfo MapToMeasurementDTO(Measurement measurement)
    {
        return new MeasurementInfo(measurement.MeasurementId, measurement.Name);
    }
    #endregion Map Model To DTO

    #region Map DTO to Model
    private static void MapRecipeDTOToRecipe(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        existingRecipe.Title = recipeDTO.Title;
        existingRecipe.CategoryId = recipeDTO.Category.Id;
        existingRecipe.ShortDescription = recipeDTO.ShortDescription;
        existingRecipe.Description = recipeDTO.Description;
        existingRecipe.Directions = recipeDTO.Directions;
        existingRecipe.PreparationTime = recipeDTO.PreparationTime;
        existingRecipe.CookingTime = recipeDTO.CookingTime;
    }
    private static Image MapImageDataDTOToImage(ImageData dto)
    {
        var model = new Image()
        {
            Id = dto.Id,
            Order = dto.Order,
            Name = dto.Name,
            Title = dto.Title,
            Data = dto.Data
        };
        return model;
    }
    #endregion  Map DTO to Model
}
