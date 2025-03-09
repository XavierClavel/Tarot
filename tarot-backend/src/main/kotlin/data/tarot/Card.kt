package xclavel.data.tarot

import xclavel.data.server.Player

data class Card(
    val id: Int,
    val value: Int,
    val color: Color,
    var owner: Player? = null,
    val status: CardStatus = CardStatus.IN_HAND,
) {

    companion object {
        fun fromId(id: Int, owner: Player?) : Card {
            val card = when (id) {
                in 1..14 -> Card(id, id, Color.CARREAU)
                in 15..28 -> Card(id, id - 14, Color.TREFLE)
                in 29..42 -> Card(id, id - 28, Color.COEUR)
                in 43..56 -> Card(id, id - 42, Color.PIQUE)
                in 57..77 -> Card(id, id - 56, Color.ATOUT)
                78 -> Card(id, -1, Color.EXCUSE)
                else -> throw Exception("Invalid card id")
            }
            card.owner = owner
            return card
        }
    }



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

    fun isExcuse() = value == -1

    fun isRegularCard() = color != Color.ATOUT && color != Color.EXCUSE
}
