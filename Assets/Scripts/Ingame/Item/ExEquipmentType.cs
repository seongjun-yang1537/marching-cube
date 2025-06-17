namespace Ingame
{
    public static class ExEquipmentType
    {
        public static EquipmentSlotID ToSlotID(this EquipmentType type)
        {
            return type switch
            {
                EquipmentType.Boots => EquipmentSlotID.Boots,
                EquipmentType.Armor => EquipmentSlotID.Armor,
                EquipmentType.Ring => EquipmentSlotID.Ring,
                EquipmentType.Helmet => EquipmentSlotID.Helmet,
                EquipmentType.Pants => EquipmentSlotID.Pants,
                EquipmentType.Pendant => EquipmentSlotID.Pendant,
                _ => (EquipmentSlotID)(-1),
            };
        }
    }
}