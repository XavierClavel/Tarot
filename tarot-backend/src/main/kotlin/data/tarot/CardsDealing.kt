package xclavel.data.tarot

import xclavel.data.server.Player

data class CardsDealing(
    val hands: HashMap<Player, MutableList<Int>> = HashMap(),
    val publicCards: HashMap<Player, MutableList<Int>> = HashMap(),
    val hiddenCards: HashMap<Player, MutableList<Int>> = HashMap(),
    val dog: MutableList<Int> = mutableListOf(),
) {
    fun toList(): List<List<Int>> = hands.values + listOf(dog)
}
