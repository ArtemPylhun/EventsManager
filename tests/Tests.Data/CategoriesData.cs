﻿using Domain.Categories;

namespace Tests.Data;

public class CategoriesData
{
    public static Category MainCategory =>
        Category.New(CategoryId.New(), "Main category name", "Main category description");
    public static Category SecondaryCategory =>
        Category.New(CategoryId.New(), "Secondary category name", "Secondary category description");
}