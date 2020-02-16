using Eagle_Island;

namespace TAS.Save
{
    public class Upgrades
    {
        public bool Glove { get; set; }
        public int Feather { get; set; }
        public bool ZepharasFeather { get; set; }
        public bool IcorasFeather { get; set; }
        public bool MagirasFeather { get; set; }
        public bool Swim { get; set; }
        public bool DoubleJump { get; set; }
        public bool ChargeAttack { get; set; }
        public bool FlashDash { get; set; }
        public bool Stratosphere { get; set; }
        public bool BlastOff { get; set; }
        public bool MagirasFury { get; set; }
        public bool IcorasWrath { get; set; }
        public bool ZepharasRage { get; set; }
        public int PerkSlots { get; set; }
        public static void Load(Upgrades upgrades)
        {
            Raven.Glove = upgrades.Glove;
            Raven.Feather = upgrades.Feather;
            Raven.ElectricFeather = upgrades.ZepharasFeather;
            Raven.IceFeather = upgrades.IcorasFeather;
            Raven.FireFeather = upgrades.MagirasFeather;
            
            Raven.ChargeUp = upgrades.ChargeAttack;
            Raven.ElectricCharge = upgrades.ZepharasRage;
            Raven.IceCharge = upgrades.IcorasWrath;
            Raven.FireCharge = upgrades.MagirasFury;

            Raven.DoubleJump = upgrades.DoubleJump;
            Raven.ElectricDoubleJump = upgrades.FlashDash;
            Raven.IceDoubleJump = upgrades.Stratosphere;
            Raven.FireDoubleJump = upgrades.BlastOff;

            Raven.Swim = upgrades.Swim;

            int extraPerkSlots = upgrades.PerkSlots - 4;
            if (extraPerkSlots > 0)
            {
                Raven.SetStuff(36, 1);
                extraPerkSlots--;
            }
            if (extraPerkSlots > 0)
            {
                Raven.SetStuff(37, 1);
                extraPerkSlots--;
            }
            if (extraPerkSlots > 0)
            {
                Raven.SetStuff(38, 1);
            }
        }
    }
}
