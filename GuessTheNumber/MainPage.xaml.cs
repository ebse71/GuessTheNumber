using System;
using Microsoft.Maui.Controls;

namespace GuessTheNumber
{
    public partial class MainPage : ContentPage
    {
        private string secretNumber; // Geheime Nummer, die erraten werden muss
        private int attempts; // Anzahl der bisherigen Versuche
        private const int maxAttempts = 10; // Maximale Anzahl der Versuche

        public MainPage()
        {
            InitializeComponent();
            secretNumber = GenerateSecretNumber(); // Neue geheime Nummer erzeugen
            attempts = 0; // Versuche auf 0 setzen
        }

        private List<string> guessHistory = new List<string>(); // Liste zur Speicherung der Ratengeschichte

        private void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            string guess = GuessEntry.Text; // Eingegebene Zahl vom Benutzer

            // Überprüfung, ob der Benutzer die Zahl mit 0 beginnt
            if (guess.StartsWith("0"))
            {
                ResultLabel.Text = "0 cannot be used as the first digit.";
                return; // Beenden, wenn die Zahl mit 0 beginnt
            }

            // Überprüfung, ob die Zahl vierstellig und numerisch ist
            if (guess.Length == 4 && int.TryParse(guess, out _))
            {
                attempts++; // Erhöhe die Anzahl der Versuche

                // Berechnung der verbleibenden Versuche
                int remainingAttempts = maxAttempts - attempts;
                if (remainingAttempts == 1)
                {
                    AttemptsLabel.Text = "This is your last chance to guess the number!";
                }
                else
                {
                    AttemptsLabel.Text = $"Remaining Attempts: {remainingAttempts}";
                }

                string feedback = CheckGuess(guess); // Überprüfung der Richtigkeit des Rates

                // Speichern der Rate und des Ergebnisses in der Liste
                guessHistory.Add($" {guess} ---> {feedback}");

                // Anzeige der bisherigen Raten in der ResultLabel
                ResultLabel.Text = string.Join("\n", guessHistory);

                // Löschen des Eingabefeldes für die nächste Eingabe
                GuessEntry.Text = "";

                // Überprüfung, ob das Spiel vorbei ist (entweder gewonnen oder maximale Versuche erreicht)
                if (feedback == "Correct! You win!" || attempts >= maxAttempts)
                {
                    ResultLabel.Text += $"\nGame Over. Secret number was: {secretNumber}";

                    SubmitButton.IsEnabled = false; // Submit-Button deaktivieren
                    SubmitButton.IsVisible = false; // Submit-Button ausblenden
                    NewGameButton.IsVisible = true; // New Game-Button anzeigen
                }
            }
            else
            {
                // Anzeige einer Fehlermeldung, wenn die Eingabe ungültig ist
                ResultLabel.Text = "Please enter a valid 4-digit number.";
            }
        }

        // Methode zur Erzeugung der geheimen Nummer (ohne doppelte Ziffern und keine 0 an erster Stelle)
        private string GenerateSecretNumber()
        {
            Random random = new Random();
            List<int> digits = new List<int>();

            // Die erste Ziffer muss zwischen 1 und 9 liegen
            digits.Add(random.Next(1, 10));

            // Die restlichen Ziffern müssen eindeutig sein und dürfen 0 enthalten
            while (digits.Count < 4)
            {
                int nextDigit = random.Next(0, 10);
                if (!digits.Contains(nextDigit)) // Keine doppelten Ziffern
                {
                    digits.Add(nextDigit);
                }
            }

            // Die Ziffern zu einer Zeichenkette zusammenfügen
            return string.Join("", digits);
        }

        // Methode zur Überprüfung der neuen Rate
        private string CheckGuess(string guess)
        {
            int correctPosition = 0; // Anzahl der Ziffern, die an der richtigen Position sind
            int correctNumberWrongPosition = 0; // Anzahl der Ziffern, die richtig sind, aber an der falschen Position

            // Überprüfung der eingegebenen Zahl
            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secretNumber[i])
                {
                    correctPosition++; // Richtig positionierte Ziffer
                }
                else if (secretNumber.Contains(guess[i]))
                {
                    correctNumberWrongPosition++; // Richtige Ziffer an falscher Position
                }
            }

            // Wenn alle Ziffern an der richtigen Position sind, ist das Spiel gewonnen
            if (correctPosition == 4)
            {
                return "Correct! You win!";
            }
            else
            {
                // Rückmeldung der richtigen und falsch positionierten Ziffern
                return $"+{correctPosition}, -{correctNumberWrongPosition}";
            }
        }

        // Methode zur Neuerstellung des Spiels (Start einer neuen Runde)
        private void OnNewGameButtonClicked(object sender, EventArgs e)
        {
            secretNumber = GenerateSecretNumber(); // Neue geheime Nummer erzeugen

            guessHistory.Clear(); // Die Ratengeschichte leeren

            ResultLabel.Text = ""; // Anzeige des Ergebnisses zurücksetzen

            attempts = 0; // Versuche zurücksetzen

            AttemptsLabel.Text = "Remaining Attempts: 10"; // Anzeige der Versuche zurücksetzen

            // Submit-Button aktivieren und sichtbar machen
            SubmitButton.IsVisible = true;
            SubmitButton.IsEnabled = true;

            // New Game-Button ausblenden
            NewGameButton.IsVisible = false;

            GuessEntry.Text = ""; // Eingabefeld leeren
            GuessEntry.IsEnabled = true; // Eingabefeld aktivieren
        }
    }
}
