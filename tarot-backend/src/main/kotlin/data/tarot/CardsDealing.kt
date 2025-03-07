package xclavel.data.tarot

import xclavel.data.server.Player

data class CardsDealing(
    val hands: HashMap<Player, MutableList<Int>>,
    val dog: MutableList<Int>,
)
