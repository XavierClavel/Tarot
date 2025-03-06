package xclavel.data.tarot

import xclavel.data.server.Player

data class Card(
    val value: Int,
    val color: Color,
    val id: Int,
    val status: CardStatus = CardStatus.IN_HAND,
    var owner: Player? = null
) {
    fun countPoint(): Float {
        return if (isOudler()) {
            4.5f //bouts
        } else if (color == Color.ATOUT) {
             0.5f
        } else {
            when (value) {
                11 -> 1.5f //valet
                12 -> 2.5f //cavalier
                13 -> 3.5f //dame
                14 -> 4.5f //roi
                else -> 0.5f //carte quelconque
            }
        }
    }

    fun isOudler() =
        color == Color.EXCUSE ||
        color == Color.ATOUT && (
            value == 1 ||
            value == 21
        )

    fun isValet() = value == 11
    fun isCavalier() = value == 12
    fun isDame() = value == 13
    fun isRoi() = value == 14
}
