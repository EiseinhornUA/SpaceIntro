[System.Serializable]
public class PhraseCharacterPair
{
    public string phrase;
    public DialogueCharacter character;
    public PhraseCharacterPair(string phrase, DialogueCharacter character)
    {
        this.phrase = phrase;
        this.character = character;
    }
}