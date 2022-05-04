using System;

public class Product : Element
{
    public Product()
    {
        Name = "NewProduct";
        ElementType = ElementType.Product;
        SubElementType = ElementType.Molecule;
    }
}