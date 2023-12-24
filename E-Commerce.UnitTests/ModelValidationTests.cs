using System.ComponentModel.DataAnnotations;
using ECommerce.Models.IdentityModels;


namespace EShop_Testing;

public class ModelValidationTests
{
    //Product:
    
    [Fact]
    public void Product_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new Product
        {
            ProductId = 1, 
            Price = 1, 
            CategoryId = 1, 
            Category = null, 
            ShortDescription = "ShortDescription",
            Images = null,
            Name = "Laptop",
            ProductDetails = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    [Fact]
    public void Product_ShouldNotPassValidation_WhenNameAndDescriptionIsEmpty()
    {
        var model = new Product
        {
            // empty
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(2,validationResult.Count);
    }

    //ProductAddDTO:
    
    [Fact]
    public void ProductAddDTO_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new ProductAddDTO
        {
            Price = 1, 
            CategoryId = 1,
            ShortDescription = "ShortDescription",
            Name = "Laptop",
            ImageFiles = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void ProductAddDTO_ShouldNotPassValidation_WhenNameAndDescriptionIsIsNull()
    {
        var model = new ProductAddDTO
        {
            Price = 1, 
            CategoryId = 1,
            ShortDescription = null,
            Name = null,
            ImageFiles = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(2,validationResult.Count);
    }
    
   
    
    
    
    
    
    
    [Fact]
    public void ProductAddDTO_ShouldNotPassValidation_WhenPriceNotPositive()
    {
        var model = new ProductAddDTO
        {
            Price = -1, 
            CategoryId = 1,
            ShortDescription = "ShortDescription",
            Name = "Name",
            ImageFiles = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
   
    }

    [Fact]
    public void ProductAddDTO_ShouldNotPassValidation_WhenShortDescriptionMinimumLengthNot10()
    {
        var model = new ProductAddDTO
        {
            Price = 1, 
            CategoryId = 1,
            ShortDescription = "1",
            Name = "Name",
            ImageFiles = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    //Category:
    
    [Fact]
    public void Category_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new Category
        {
            CategoryId = 1,
            CategoryName = "Laptop"
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void ProductAddDTO_ShouldNotPassValidation_WhenCategoryNameIsNull()
    {
        var model = new Category
        {
            CategoryId = 1,
            CategoryName = null,
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }

    //Image:
    
    [Fact]
    public void Image_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new Image
        {
            ImageBytes = new byte[]{1,2,1,1}
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void PImage_ShouldNotPassValidation_WhenImageBytesIsNull()
    {
        var model = new Image
        {
             
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    //ProductDetail:
    
    [Fact]
    public void ProductDetail_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new ProductDetail
        {
            Title = "SomeTitle",
            Description = "SomeDescription",
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void ProductDetail_ShouldNotPassValidation_WhenTitleAndDescriptionIsNull()
    {
        var model = new ProductDetail
        {
             
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(2,validationResult.Count);
    }
    
    //LoginDTO:
    [Fact]
    public void LoginDTO_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new LoginDTO
        {
            Email = "Email@email.com",
            Password = "123",
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void ProductDetail_ShouldNotPassValidation_WhenEmailAndPasswordIsNull()
    {
        var model = new LoginDTO
        {
            
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(2,validationResult.Count);
    }
    
    [Fact]
    public void ProductDetail_ShouldNotPassValidation_WhenEmailIsIncorrect()
    {
        var model = new LoginDTO
        {
            Email = "IncorrectEmail"
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    //RegistrationDTO:
    
    [Fact]
    public void RegistrationDTO_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new RegistrationDTO
        {
            UserName = "Dima",
            Password = "123",
            ConfirmPassword = "123",
            Email = "test@test.com",
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void ProductDetail_ShouldNotPassValidation_WhenAllPropIsNull()
    {
        var model = new RegistrationDTO
        {
            
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(4,validationResult.Count);
    }
    
    [Fact]
    public void ProductDetail_ShouldNotPassValidation_WhenPasswordDontMatch()
    {
        var model = new RegistrationDTO
        {
            UserName = "Dima",
            Password = "123",
            ConfirmPassword = "321",
            Email = "test@test.com",
        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    //Order:
    [Fact]
    public void Order_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new Order
        {
            OrderId = 1,
            Adress = "SomeAdress",
            PhoneNumber = "+380506165467",
            Status = OrderStatusEnum.OrderStatus.Accepted.ToString(),
            UserId = "someUserId123"

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void Order_ShouldNotPassValidation_WhenAllPropIsNull()
    {
        var model = new Order
        {
             

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(3,validationResult.Count);
    }
    
    [Fact]
    public void Order_ShouldNotPassValidation_WhenPhoneNumberIsIncorrect()
    {
        var model = new Order
        {
            OrderId = 1,
            Adress = "SomeAdress",
            PhoneNumber = "6165467",
            Status = OrderStatusEnum.OrderStatus.Accepted.ToString(),
            UserId = "someUserId123"

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    //OrderAddDTO:
    [Fact]
    public void OrderAddDTO_ShouldPassValidation_WhenModelIsValid()
    {
        var model = new OrderAddDTO
        {
            Adress = "SomeAdress",
            PhoneNumber = "+380506165467",
            Status = OrderStatusEnum.OrderStatus.Accepted.ToString(),
            TotalPrice = 1,

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Empty(validationResult);
    }
    
    [Fact]
    public void OrderAddDTO_ShouldNotPassValidation_WhenAllPropIsNull()
    {
        var model = new OrderAddDTO
        {
             

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.Equal(3,validationResult.Count);
    }
    
    //OrderItem:
    [Fact]
    public void OrderItem_ShouldNotPassValidation_WhenQuantityIsOutOfRange()
    {
        var model = new OrderItem
        {
            Quantity = 111,

        };
        var validationResult = ModelValidationTests.ValidateModel(model);
        Assert.NotEmpty(validationResult);
    }
    
    
    
    public static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();

        var validationContext = new ValidationContext(model, null, null);

        Validator.TryValidateObject(model, validationContext, results, true);

        if (model is IValidatableObject validatableModel)
            results.AddRange(validatableModel.Validate(validationContext));

        return results;
    }

}
