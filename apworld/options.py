from dataclasses import dataclass
from Options import Choice, PerGameCommonOptions


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


@dataclass
class ScritchyScratchyOptions(PerGameCommonOptions):
    coin_mode: CoinMode
