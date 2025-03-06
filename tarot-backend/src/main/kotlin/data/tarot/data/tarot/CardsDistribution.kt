package xclavel.data.tarot.data.tarot

import xclavel.data.server.Player
import xclavel.data.tarot.Card
import xclavel.data.tarot.CardStatus
import xclavel.data.tarot.Color

data class CardsDistribution(
    val hands: Pair<Player, List<Card>>,
    val hiddenHands: Pair<Player, List<Card>>,
    val visibleHands: Pair<Player, List<Card>>,
    val dog: List<Card>,
)
