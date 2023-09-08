/// <summary>
/// Defines equipment for use with the weaponSystem
/// </summary>
public interface IEquipment
{
    /// <summary>
    /// The name of the equipment
    /// </summary>
    /// <returns></returns>
    string EquipmentName();
    /// <summary>
    /// Function performed when using the equipment
    /// </summary>
    void Use();
}
