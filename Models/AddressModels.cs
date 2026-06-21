namespace ERPHub.Models;

public class AddressDivision
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class AddressDistrict
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DivisionId { get; set; }
}

public class AddressUpazila
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DistrictId { get; set; }
}
