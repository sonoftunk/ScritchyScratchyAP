from dataclasses import dataclass
from Options import Choice, PerGameCommonOptions, Toggle


class CoinMode(Choice):
    """
    How coin upgrades are added to the item pool.
    individual: each coin (Tin, Aluminum, ...) is its own item and can arrive in any order.
    progressive: a "Progressive Coin" item unlocks the next coin in order
    (Tin, Aluminum, Copper, Bronze, Iron, Steel, Titanium, Tungsten).
    """
    display_name = "Coin Mode"
    option_individual = 0
    option_progressive = 1
    default = 1


class JunkSuperTicketChecks(Toggle):
    """Replace Super ticket cash-out rewards with cash-injection filler."""
    display_name = "Junk Super Ticket Checks"
    default = 0


@dataclass
class ScritchyScratchyOptions(PerGameCommonOptions):
    coin_mode: CoinMode
    junk_super_ticket_checks: JunkSuperTicketChecks
