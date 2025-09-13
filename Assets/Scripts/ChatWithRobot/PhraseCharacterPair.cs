[System.Serializable]
public class PhraseCharacterPair
{
    public string phrase { get; set; }
    public DialogueCharacter character { get; set; }
    public PhraseCharacterPair(string phrase, DialogueCharacter character)
    {
        this.phrase = phrase;
        this.character = character;
    }
}