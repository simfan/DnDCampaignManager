using BlazorApp1.Models;

namespace BlazorApp1.Services
{
    public class DiceService
    {
        private readonly Random _random = new Random();

        // Basic roll method
        public DiceRoll Roll(int numberOfDice, DiceType diceType, int modifier = 0, string rollType = "Custom")
        {
            var results = new List<int>();

            for (int i = 0; i < numberOfDice; i++)
            {
                results.Add(_random.Next(1, (int)diceType + 1));
            }

            var total = results.Sum();
            var finalTotal = total + modifier;

            return new DiceRoll
            {
                RollType = rollType,
                Results = results,
                Total = total,
                Modifier = modifier,
                FinalTotal = finalTotal,
                DiceNotation = $"{numberOfDice}d{(int)diceType}{(modifier >= 0 ? "+" : "")}{modifier}",
                Timestamp = DateTime.UtcNow
            };
        }

        // Parse dice notation (e.g., "2d20+5") and roll
        public DiceRoll RollFromNotation(string notation, string rollType = "Custom")
        {
            try
            {
                // Remove spaces
                notation = notation.Replace(" ", "").ToLower();

                // Parse the notation
                int numberOfDice = 1;
                int diceSize = 20;
                int modifier = 0;

                // Split by 'd'
                var parts = notation.Split('d');
                if (parts.Length == 2)
                {
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        numberOfDice = int.Parse(parts[0]);
                    }

                    // Check for modifier
                    var diceSizeAndModifier = parts[1];
                    if (diceSizeAndModifier.Contains('+'))
                    {
                        var modParts = diceSizeAndModifier.Split('+');
                        diceSize = int.Parse(modParts[0]);
                        modifier = int.Parse(modParts[1]);
                    }
                    else if (diceSizeAndModifier.Contains('-'))
                    {
                        var modParts = diceSizeAndModifier.Split('-');
                        diceSize = int.Parse(modParts[0]);
                        modifier = -int.Parse(modParts[1]);
                    }
                    else
                    {
                        diceSize = int.Parse(diceSizeAndModifier);
                    }
                }

                // Find matching DiceType
                DiceType diceType = diceSize switch
                {
                    4 => DiceType.D4,
                    6 => DiceType.D6,
                    8 => DiceType.D8,
                    10 => DiceType.D10,
                    12 => DiceType.D12,
                    20 => DiceType.D20,
                    100 => DiceType.D100,
                    _ => throw new ArgumentException($"Unsupported dice size: {diceSize}")
                };

                return Roll(numberOfDice, diceType, modifier, rollType);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Invalid dice notation: {notation}");
            }
        }

        // Advantage roll (roll twice, take higher)
        public DiceRoll RollWithAdvantage(DiceType diceType, int modifier = 0, string rollType = "Skill Check")
        {
            var roll1 = _random.Next(1, (int)diceType + 1);
            var roll2 = _random.Next(1, (int)diceType + 1);
            var higher = Math.Max(roll1, roll2);

            return new DiceRoll
            {
                RollType = $"{rollType} (Advantage)",
                Results = new List<int> { roll1, roll2 },
                Total = higher,
                Modifier = modifier,
                FinalTotal = higher + modifier,
                DiceNotation = $"2d{(int)diceType} (advantage) {(modifier >= 0 ? "+" : "")}{modifier}",
                Timestamp = DateTime.UtcNow
            };
        }

        // Disadvantage roll (roll twice, take lower)
        public DiceRoll RollWithDisadvantage(DiceType diceType, int modifier = 0, string rollType = "Skill Check")
        {
            var roll1 = _random.Next(1, (int)diceType + 1);
            var roll2 = _random.Next(1, (int)diceType + 1);
            var lower = Math.Min(roll1, roll2);

            return new DiceRoll
            {
                RollType = $"{rollType} (Disadvantage)",
                Results = new List<int> { roll1, roll2 },
                Total = lower,
                Modifier = modifier,
                FinalTotal = lower + modifier,
                DiceNotation = $"2d{(int)diceType} (disadvantage) {(modifier >= 0 ? "+" : "")}{modifier}",
                Timestamp = DateTime.UtcNow
            };
        }

        // Specific roll types
        public DiceRoll SkillCheck(int modifier = 0, bool advantage = false, bool disadvantage = false)
        {
            if (advantage)
                return RollWithAdvantage(DiceType.D20, modifier, "Skill Check");
            else if (disadvantage)
                return RollWithDisadvantage(DiceType.D20, modifier, "Skill Check");
            else
                return Roll(1, DiceType.D20, modifier, "Skill Check");
        }

        public DiceRoll SavingThrow(int modifier = 0, bool advantage = false, bool disadvantage = false)
        {
            if (advantage)
                return RollWithAdvantage(DiceType.D20, modifier, "Saving Throw");
            else if (disadvantage)
                return RollWithDisadvantage(DiceType.D20, modifier, "Saving Throw");
            else
                return Roll(1, DiceType.D20, modifier, "Saving Throw");
        }

        public DiceRoll AttackRoll(int modifier = 0, bool advantage = false, bool disadvantage = false)
        {
            if (advantage)
                return RollWithAdvantage(DiceType.D20, modifier, "Attack Roll");
            else if (disadvantage)
                return RollWithDisadvantage(DiceType.D20, modifier, "Attack Roll");
            else
                return Roll(1, DiceType.D20, modifier, "Attack Roll");
        }

        public DiceRoll Initiative(int modifier = 0)
        {
            return Roll(1, DiceType.D20, modifier, "Initiative");
        }

        public DiceRoll DamageRoll(int numberOfDice, DiceType diceType, int modifier = 0)
        {
            return Roll(numberOfDice, diceType, modifier, "Damage Roll");
        }
    }
}