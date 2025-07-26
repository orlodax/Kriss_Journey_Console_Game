namespace KrissJourney.Kriss.Models;

/// <summary>
/// Represents how strong Kriss is.
/// </summary>
/// <param name="Health">Your HP.</param>
/// <param name="BaseDamage">Your attack power.</param>
/// <param name="RageBonus">Your rage bonus. It is meant to be a function of how often you get hit.</param>
/// <param name="FuryBonus">Your fury bonus. It is meant to trigger when you or a party member reaches 10% health, or other narratively major distressing situation.</param>
public record struct Prowess(
    int Health,
    int BaseDamage,
    int RageBonus,
    int FuryBonus);
