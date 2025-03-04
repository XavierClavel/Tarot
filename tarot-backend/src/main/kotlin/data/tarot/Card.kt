package xclavel.data.tarot

import xclavel.data.server.Player

data class Card(
    val value: Int,
    val color: Color,
    val id: Long,
    val status: CardStatus = CardStatus.IN_HAND,
    var owner: Player? = null
) {
    fun countPoint(): Float {
        return if (color == Color.ATOUT) {
            if (isOudler()) {
                4.5f //bouts
            } else 0.5f
        } else {
            when (value) {
                11 -> 1.5f //valet
                12 -> 2.5f //cavalier
                13 -> 3.5f //dame
                14 -> 4.5f //roi
                else -> 0.5f
            }
        }
    }

    fun isOudler() = color == Color.ATOUT && (
            value == -1 ||
            value == 1 ||
            value == 21
            )
}
