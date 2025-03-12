package xclavel.data.tarot

enum class Color {
    CARREAU,
    COEUR,
    TREFLE,
    PIQUE,
    ATOUT,
    EXCUSE;

    fun isRegularColor() = this == CARREAU || this == COEUR || this == TREFLE || this == PIQUE
}