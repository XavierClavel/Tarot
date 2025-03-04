package xclavel.data.tarot

import xclavel.data.server.Player

data class Card(
    val value: Int,
    val color: Color,
    val id: Long,
    val status: CardStatus = CardStatus.IN_HAND,
    var owner: Player? = null
)
