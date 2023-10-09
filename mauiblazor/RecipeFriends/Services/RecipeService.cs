using Microsoft.EntityFrameworkCore;
using RecipeFriends.Shared.Data;
using RecipeFriends.Shared.Data.Models;
using RecipeFriends.Shared.DTO;
using Image = RecipeFriends.Shared.Data.Models.Image;

namespace RecipeFriends.Services;

public class RecipeService : IRecipeService
{
    private readonly RecipeFriendsDbContext _context;

    public RecipeService(RecipeFriendsDbContext context)
    {
        _context = context;
    }

    public async Task<RecipeDetails> GetRecipeDetailsAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return null;
        }
        ;
        return MapToRecipeDetailsDTO(recipe);
    }

    public async Task<ImageData> GetImageDataAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
        {
            return null;
        }
        ;
        return MapToImageDataDTO(image);
    }

    public async Task<RecipeInfo[]> GetRecipesAsync(CancellationToken cancellationToken)
    {
        var l = await _context.Recipes.Include(r => r.Tags).ToListAsync();
        var r = l.Select(MapToRecipeInfoDTO).ToArray();
        return r;
    }

    public async Task<bool> SaveRecipeDetailsAsync(RecipeDetails recipeDTO, CancellationToken cancellationToken)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                if (recipeDTO.Id > 0)
                {
                    if (!await UpdateRecipeAsync(recipeDTO, cancellationToken))
                        return false;
                }
                else
                {
                    InsertNewRecipe(recipeDTO);
                }
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken); // Commit the transaction if all operations succeeded
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken); // Rollback any changes made during the transaction
                if (!_context.Recipes.Any(e => e.Id == recipeDTO.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken); // Rollback for any other exception
                throw; // Propagate the exception upwards
            }
        }
    }

    private async Task<bool> UpdateRecipeAsync(
        RecipeDetails recipeDTO,
        CancellationToken cancellationToken
    )
    {
        var existingRecipe = await _context.Recipes
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == recipeDTO.Id, cancellationToken);

        if (existingRecipe == null)
            return false;

        MapRecipeDTOToRecipe(recipeDTO, existingRecipe);

        UpdateRecipeTags(recipeDTO, existingRecipe);
        UpdateRecipeEquipment(recipeDTO, existingRecipe);
        UpdateRecipeIngredients(recipeDTO, existingRecipe);
        return true;
    }

    private void InsertNewRecipe(RecipeDetails recipeDTO)
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
        _context.Recipes.Add(newRecipe);
        UpdateRecipeTags(recipeDTO, newRecipe);
        UpdateRecipeEquipment(recipeDTO, newRecipe);
        UpdateRecipeIngredients(recipeDTO, newRecipe);
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

    private void UpdateRecipeIngredients(RecipeDetails recipeDTO, Recipe existingRecipe)
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

    public async Task<bool> SaveImageDataAsync(int recipeId, ImageData img, CancellationToken cancellationToken){

        var recipe = await _context.Recipes.FindAsync(recipeId, cancellationToken);

        if (recipe == null)
            return false;

        recipe.Images.Add(MapImageDataDTOToImage(img));
        await _context.SaveChangesAsync(cancellationToken);
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

    private ImageData MapToImageDataDTO(Image image){
        var result = new ImageData() {
            Id = image.Id,
            Name = image.Name,
            Title = image.Title,
            Data = image.Data
        };
        return result;
    }


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
                    rt =>
                        new ImageInfo()
                        {
                            Id = rt.Id,
                            Title = rt.Title,
                            Name = rt.Name
                        }
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
        return new MeasurementInfo { Id = measurement.MeasurementId, Name = measurement.Name };
    }
    #endregion Map Model To DTO

    #region Map DTO to Model
    private void MapRecipeDTOToRecipe(RecipeDetails recipeDTO, Recipe existingRecipe)
    {
        existingRecipe.Title = recipeDTO.Title;
        existingRecipe.CategoryId = recipeDTO.Category.Id;
        existingRecipe.ShortDescription = recipeDTO.ShortDescription;
        existingRecipe.Description = recipeDTO.Description;
        existingRecipe.Directions = recipeDTO.Directions;
        existingRecipe.PreparationTime = recipeDTO.PreparationTime;
        existingRecipe.CookingTime = recipeDTO.CookingTime;
    }
    private Image MapImageDataDTOToImage(ImageData dto)
    {
        var model = new Image() {
            Id = dto.Id,
            Name = dto.Name,
            Title = dto.Title,
            Data = dto.Data
        };
        return model;
    }
    #endregion  Map DTO to Model
}
