using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryItemTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbilityUsed",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmmunitionType",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArmorClassBonus",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArmorType",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssociatedSkill",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttackType",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseArmorClass",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarryingCapacity",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentCharges",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageBonus",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageBonusOverride",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageDiceCount",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageDiceSides",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageType",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Effect",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasProficiency",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasStealth",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealBonus",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealDiceCount",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealDiceSides",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttuned",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLight",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsThrown",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTwoHanded",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCharges",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxDexBonus",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MountType",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PotionItem_Effect",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangeLongFt",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangeNormalFt",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rarity",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReachFt",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAttunement",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpeedFt",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StealthDisadvantage",
                table: "InventoryItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StrengthRequirement",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToolType",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersatileDiceCount",
                table: "InventoryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersatileDiceSides",
                table: "InventoryItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbilityUsed",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "AmmunitionType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ArmorClassBonus",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ArmorType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "AssociatedSkill",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "AttackType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "BaseArmorClass",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CarryingCapacity",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CurrentCharges",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DamageBonus",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DamageBonusOverride",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DamageDiceCount",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DamageDiceSides",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DamageType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Effect",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HasProficiency",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HasStealth",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HealBonus",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HealDiceCount",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HealDiceSides",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsAttuned",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsLight",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsThrown",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsTwoHanded",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "MaxCharges",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "MaxDexBonus",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "MountType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "PotionItem_Effect",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Properties",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "RangeLongFt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "RangeNormalFt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ReachFt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "RequiresAttunement",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "SpeedFt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "StealthDisadvantage",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "StrengthRequirement",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ToolType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "VersatileDiceCount",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "VersatileDiceSides",
                table: "InventoryItems");
        }
    }
}
