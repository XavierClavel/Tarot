public class Card
{
    public readonly int index;
    public readonly int value;
    public readonly TarotColor color;

    public bool isRoi() => value == 14;
    public bool isReine() => value == 13;
    public bool isCavalier() => value == 12;
    public bool isValet() => value == 11;
    public bool isExcuse() => value == -1;

    public string toString()
    {
        if (isExcuse()) return "Excuse";
        if (color == TarotColor.ATOUT) return $"{value} d'atout";
        if (isRoi()) return $"Roi de {color}";
        if (isReine()) return $"Reine de {color}";
        if (isCavalier()) return $"Cavalier de {color}";
        if (isValet()) return $"Valet de {color}";
        return $"{value} de {color}";
    }
    
    public Card(int index)
    {
        this.index = index;
        switch (index)
        {
            case <= 14:
                this.color = TarotColor.CARREAU;
                this.value = index;
                break;
            
            case <= 28:
                this.color = TarotColor.TREFLE;
                this.value = index - 14;
                break;
            
            case <= 42:
                this.color = TarotColor.COEUR;
                this.value = index - 28;
                break;
            
            case <= 56:
                this.color = TarotColor.PIQUE;
                this.value = index - 42;
                break;
            
            case <= 77:
                this.color = TarotColor.ATOUT;
                this.value = index - 56;
                break;
            
            case 78:
                this.color = TarotColor.EXCUSE;
                this.value = -1;
                break;
        }
    }
}
