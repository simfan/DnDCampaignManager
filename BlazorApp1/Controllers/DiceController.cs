using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers
{
    /// <summary>
    /// Dice roll API endpoints consumed by the Stream Deck plugin.
    /// No authentication required so the plugin can call without a session token.
    /// Scope access via CORS to localhost only (configured in Program.cs).
    /// </summary>
    [Route("api/dice")]
    [ApiController]
    public class DiceController : ControllerBase
    {
        private readonly DiceService _diceService;

        public DiceController(DiceService diceService)
        {
            _diceService = diceService;
        }

        // POST /api/dice/skillcheck
        [HttpPost("skillcheck")]
        public ActionResult<DiceRoll> SkillCheck([FromBody] D20RollRequest request)
        {
            var result = _diceService.SkillCheck(request.Modifier, request.Advantage, request.Disadvantage);
            return Ok(result);
        }

        // POST /api/dice/savingthrow
        [HttpPost("savingthrow")]
        public ActionResult<DiceRoll> SavingThrow([FromBody] D20RollRequest request)
        {
            var result = _diceService.SavingThrow(request.Modifier, request.Advantage, request.Disadvantage);
            return Ok(result);
        }

        // POST /api/dice/attackroll
        [HttpPost("attackroll")]
        public ActionResult<DiceRoll> AttackRoll([FromBody] D20RollRequest request)
        {
            var result = _diceService.AttackRoll(request.Modifier, request.Advantage, request.Disadvantage);
            return Ok(result);
        }

        // POST /api/dice/initiative
        [HttpPost("initiative")]
        public ActionResult<DiceRoll> Initiative([FromBody] InitiativeRequest request)
        {
            var result = _diceService.Initiative(request.Modifier);
            return Ok(result);
        }

        // POST /api/dice/damage
        [HttpPost("damage")]
        public ActionResult<DiceRoll> Damage([FromBody] DamageRollRequest request)
        {
            if (!Enum.IsDefined(typeof(DiceType), request.DiceType))
                return BadRequest($"Invalid dice type: {request.DiceType}. Valid values: 4, 6, 8, 10, 12, 20, 100.");

            var diceType = (DiceType)request.DiceType;
            var result = _diceService.DamageRoll(request.NumberOfDice, diceType, request.Modifier);
            return Ok(result);
        }

        // POST /api/dice/custom
        [HttpPost("custom")]
        public ActionResult<DiceRoll> Custom([FromBody] CustomRollRequest request)
        {
            try
            {
                var result = _diceService.RollFromNotation(request.Notation, "Custom Roll");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // ── Request DTOs ─────────────────────────────────────────────────────────

    public class D20RollRequest
    {
        public int Modifier { get; set; } = 0;
        public bool Advantage { get; set; } = false;
        public bool Disadvantage { get; set; } = false;
    }

    public class InitiativeRequest
    {
        public int Modifier { get; set; } = 0;
    }

    public class DamageRollRequest
    {
        public int NumberOfDice { get; set; } = 1;
        public int DiceType { get; set; } = 6;  // Raw int — 4,6,8,10,12,20,100
        public int Modifier { get; set; } = 0;
    }

    public class CustomRollRequest
    {
        public string Notation { get; set; } = "1d20";
    }
}
