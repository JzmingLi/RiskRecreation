using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace RiskRecreation
{
    public enum PlayerSlot
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Unoccupied
    }

    public enum Card
    {
        Infantry,
        Cavalry,
        Artillery
    }

    public enum States
    {
        Initial,
        Strengthening,
        Attack,
        Fortification

    }

    public class Country
    {
        private PlayerSlot player;
        private int armies;
        private List<Country> connectedCountries;
        private Continent continent;

        private readonly string _countryName;
        public string Name
        {
            get { return _countryName; }
        }

        private readonly Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        private readonly GameObject _sprite;
        public GameObject Sprite
        {
            get { return _sprite; }
        }

        public Country(string name, Continent continent, Vector2 position, GameObject sprite) // Constructor with continent
        {
            player = PlayerSlot.Unoccupied;
            _countryName = name;
            armies = 0;
            connectedCountries = new List<Country>();
            this.continent = continent;
            _position = position;
            _sprite = sprite;
        }
        public Country(string name, List<Country> connections, Continent continent, Vector2 position, GameObject sprite) // Constructor with predefined connections
        {
            player = PlayerSlot.Unoccupied;
            _countryName = name;
            armies = 0;
            connectedCountries = connections;
            this.continent = continent;
            _position = position;
            _sprite = sprite;
        }

        /* Don't think this method is necessary
        public Country()
        {
            connectedCountries = new List<Country>();
        }
        */

        // Managing Connections
        public void AddConnection(Country connection)
        {
            connectedCountries.Add(connection);
        }
        public void MakeConnection(Country connection) // Ensures the country its making a connection with also adds them as a connection
        {
            if (!CheckConnection(connection))
            {
                AddConnection(connection);
            }
            if (!connection.CheckConnection(this))
            {
                connection.AddConnection(this);
            }
        }

        // Will be useful for when player is choosing a country to move/invade to
        public List<Country> GetConnections()
        {
            return connectedCountries;
        }
        public bool CheckConnection(Country connection)
        {
            return connectedCountries.Contains(connection);
        }

        // Armies
        public void AddArmies(int amount) // Add armies to country
        {
            armies += amount;

        }
        public void ClearArmies()
        {
            armies = 0;
        }
        public void SubtractArmies(int amount)
        {
            armies -= amount;
            if (armies < 0) Debug.Log("Negative Armies");
        }
        public void SetArmies(int amount)
        {
            armies = amount;
        }
        public bool TransferArmies(int amount, Country targetCountry) // For fortification phase, returns false if unable to preform action
        {
            if (amount > armies)
            {
                return false;
            }
            else
            {
                armies -= amount;
                targetCountry.AddArmies(amount);
                return true;
            }
        }
        public int Armies()
        {
            return armies; 
        }

        // Players
        public void SetPlayer(PlayerSlot player)
        {
            this.player = player;
        }
        public PlayerSlot GetPlayer() { return player; }
    }

    public class Continent
    {
        private readonly string _continentName;
        public string Name
        {
            get { return _continentName; }
        }
        private readonly int _controlValue;
        public int ControlValue
        {
            get { return _controlValue; }
        }

        private bool isControlled;
        private PlayerSlot controllingPlayer;
        private List<Country> countries;
        private readonly GameObject _sprite;
        public GameObject Sprite
        {
            get { return _sprite; }
        }

        public Continent(string name, int controlValue, List<Country> countries, GameObject sprite) // Instantiate with countries
        {
            _continentName = name;
            _controlValue = controlValue;
            controllingPlayer = PlayerSlot.Unoccupied;
            this.countries = countries;
            _sprite = sprite;
        }

        public Continent(string name, int controlValue, GameObject sprite) // Empty continent
        {
            _continentName = name;
            _controlValue = controlValue;
            controllingPlayer = PlayerSlot.Unoccupied;
            countries = new List<Country>();
            _sprite = sprite;
        }

        public void AddCountry(Country country) //Only adds if country isn't already there
        {
            if(!countries.Contains(country))
            {
                countries.Add(country);
            }
        }

        public void CheckIfControlled() //Check if all countires are controlled by the same player
        {
            PlayerSlot player = countries[0].GetPlayer(); 
            isControlled = !(player == PlayerSlot.Unoccupied);

            foreach (Country country in countries)
            {
                if (country.GetPlayer() != player)
                {
                    isControlled = false;
                }
            }

            if (isControlled)
            {
                controllingPlayer = countries[0].GetPlayer();
            }
        }

        public PlayerSlot GetPlayer()
        {
            return controllingPlayer;
        }

        public bool GetIsControlled()
        {
            return isControlled;
        }
    }

    public class Map
    {
        public List<Country> countries;
        List<Continent> continents;

        public List<GameObject> continentSprites;
        public List<GameObject> countrySprites;
        
        public Map(List<Country> setCountries, List<Continent> setContinents)
        {
            countries= setCountries;
            continents = setContinents;

            continentSprites = new List<GameObject>();
            countrySprites = new List<GameObject>();

            foreach(Country country in countries)
            {
                countrySprites.Add(country.Sprite);
            }
            foreach(Continent continent in continents)
            {
                continentSprites.Add(continent.Sprite);
            }
        }

        public Dictionary<PlayerSlot, Color> PlayerColors = new Dictionary<PlayerSlot, Color>(); // Determine player colours

        public void ChoosePlayerColors(Color player1, Color player2, Color player3, Color player4, Color unoccupied) // Called by main to assign player colors
        {
            PlayerColors.Add(PlayerSlot.Player1, player1);
            PlayerColors.Add(PlayerSlot.Player2, player2);
            PlayerColors.Add(PlayerSlot.Player3, player3);
            PlayerColors.Add(PlayerSlot.Player4, player4);
            PlayerColors.Add(PlayerSlot.Unoccupied, unoccupied);
        }

        // Instantiate all visuals and game objects (POSSILY REDUNDANT SO COMMENTED OUT FOR NOW)
        /**
        public void DrawMap()
        {
            foreach(Continent continent in continents)
            {
                continentSprites.Add(Object.Instantiate(continent.Sprite));
            }

            foreach(Country country in countries)
            {
                Vector3 positionIn3D = new Vector3(country.Position.x, country.Position.y, 0);
                countrySprites.Add(Object.Instantiate(country.Sprite, positionIn3D, Quaternion.identity));
            }
        }
        **/

        // Update Army Counters and Colors
        public void UpdateCountryLabels()
        {
            foreach(Country country in countries)
            {
                // Counter
                GameObject countrySprite = countrySprites[countries.IndexOf(country)];
                countrySprite.GetComponentInChildren<TextMeshPro>().text = country.Armies().ToString();

                //Color
                Color playerColor = PlayerColors[country.GetPlayer()];
                countrySprite.GetComponent<SpriteRenderer>().color = playerColor;
            }
        }

        // Change color of continent sprite if all countries within have the same owner
        public void ShowContinentPlayerControl()
        {
            foreach (Continent continent in continents)
            {
                continent.CheckIfControlled();
                if (continent.GetIsControlled())
                {
                    Color playerColor = PlayerColors[continent.GetPlayer()];
                    GameObject continentSprite = continentSprites[continents.IndexOf(continent)];
                    continentSprite.GetComponent<SpriteRenderer>().color = playerColor;
                }
                else
                {
                    Color defaultColor = PlayerColors[PlayerSlot.Unoccupied];
                    GameObject continentSprite = continentSprites[continents.IndexOf(continent)];
                    continentSprite.GetComponent<SpriteRenderer>().color = defaultColor;
                }
            }
        }

        // Initial Stage Stuff
        public Country GetRandomCountry()
        {
            int countryNumber = Random.Range(0, countries.Count);
            return countries[countryNumber];
        }

        public bool CheckForUnoccupiedCountries() // To avoid infinite loop 
        {
            foreach(Country country in countries)
            {
                if(country.GetPlayer() == PlayerSlot.Unoccupied)
                {
                    return true;
                }
            }
            Debug.Log("No Unoccupied Countries!");
            return false;
        }

        public Country GetStartingCountry()
        {
            Country startingCountry = GetRandomCountry();
            while (CheckForUnoccupiedCountries())
            {
                if (startingCountry.GetPlayer() != PlayerSlot.Unoccupied)
                {
                    Debug.Log("Country Occupied");
                    startingCountry = GetRandomCountry();
                }
                else
                {
                    Debug.Log("Country Unoccupied");
                     return startingCountry;
                 }
    
            }
            return null;
        }

        //Reinforcing Stage Stuff
        public int GetControlledContinents (Player player)
        {
            int controlledContinents = 0;
            foreach(Continent continent in continents)
            {
                if(continent.GetPlayer() == player.GetPlayerSlot())
                {
                    controlledContinents++;
                }
            }
            return controlledContinents;
        }
    }

    public class Player
    {
        private PlayerSlot playerSlot;
        public List<Country> countries;
        public int armies; // Should be 0 at the end of a turn
        private int exchangeCounter; //Should be 1 at the start of every turn
        private int InfantryCards;
        private int ArtilleryCards;
        private int CavalryCards;

        public Player(List<Country> countries, int playerSlot)
        {
            this.countries = countries;
            armies = 0;
            exchangeCounter = 1;
            InfantryCards = 0;
            ArtilleryCards = 0;
            CavalryCards = 0;
            this.playerSlot = SetPlayerSlot(playerSlot);
        }

        public PlayerSlot GetPlayerSlot()
        {
            return playerSlot;
        }

        public PlayerSlot SetPlayerSlot(int slot)
        {
            switch(slot)
            {
                case 0:return PlayerSlot.Player1;
                case 1: return PlayerSlot.Player2;
                case 2: return PlayerSlot.Player3;
                case 3: return PlayerSlot.Player4;
                default: 
                    Debug.Log("Player lot not in range"); 
                    return PlayerSlot.Unoccupied;
            }
        }

        // Country Methods;
        public void AddCountry(Country country)
        {
            countries.Add(country);
            country.SetPlayer(playerSlot);
        }

        public void RemoveCountry(Country country)
        {
            countries.Remove(country);
        }

        public int GetCountryCount()
        {
            return countries.Count;
        }

        public Country GetRandomCountry()
        {
            return countries[Random.Range(0, GetCountryCount())];
        }

        // Armies, not all functions necessary but do exactly as they are supposed to
        // In main, the armies will be added and then the player will subtract armies to reinforce countries
        public bool CheckArmiesDepleted()
        {
            return armies == 0;
        }

        public void AddArmies(int amount)
        {
            armies += amount;

        }

        public void SetArmies(int amount)
        {
            armies = amount;
        }

        public void SubtractArmies(int amount)
        {
            armies -= amount;
            if (armies < 0) Debug.Log("Negative Armies");
        }

        //Function to tally total armies in case
        public int GetTotalArmies()
        {
            int totalArmies = 0;
            foreach(Country country in countries)
            {
                totalArmies += country.Armies();
            }
            return totalArmies;
        }

        //Use cards to get armies
        public int ExchangeCards(Card card) // Will take 3 from the card type so check the amount of cards before running this method!!
        {
            switch (card)
            {
                case Card.Infantry:
                    InfantryCards -= 3;
                    armies += 5 * exchangeCounter;
                    exchangeCounter++;
                    return InfantryCards;

                case Card.Cavalry:
                    CavalryCards -= 3;
                    armies += 5 * exchangeCounter;
                    exchangeCounter++;
                    return CavalryCards;

                case Card.Artillery:
                    ArtilleryCards -= 3;
                    armies += 5 * exchangeCounter;
                    exchangeCounter++;
                    return ArtilleryCards;

                default:
                    Debug.Log("Invalid card type.");
                    return 0;

            }

            
        }

        public List<Card> Cards()
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < InfantryCards; i++) cards.Add(Card.Infantry);
            for (int i = 0; i < ArtilleryCards; i++) cards.Add(Card.Artillery);
            for (int i = 0; i < CavalryCards; i++) cards.Add(Card.Cavalry);

            return cards;
        }

        public Vector3 CardsAsVector()
        {
            return new Vector3(InfantryCards, ArtilleryCards, CavalryCards);
        }

        public void AddCard(Card card)
        {
            switch (card)
            {
                case Card.Infantry: 
                    InfantryCards++; 
                    break;
                case Card.Cavalry: 
                    CavalryCards++; 
                    break;
                case Card.Artillery:
                    ArtilleryCards++; 
                    break;
            }
        }

        public void AddCard()
        {
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    InfantryCards++;
                    break;
                case 1:
                    CavalryCards++;
                    break;
                case 2:
                    ArtilleryCards++;
                    break;
            }
        }


        public void RemoveCard(Card card)
        {
            switch (card)
            {
                case Card.Infantry:
                    InfantryCards--;
                    break;
                case Card.Cavalry:
                    CavalryCards--;
                    break;
                case Card.Artillery:
                    ArtilleryCards--;
                    break;
            }
        }

        public void ExchangeOneOfEachCard()
        {
            InfantryCards--;
            CavalryCards--;
            ArtilleryCards--;
            armies += 5 * exchangeCounter;
            exchangeCounter++;
        }


        public int CardCount()
        {
            return InfantryCards + CavalryCards + ArtilleryCards;
        }

        public void AddTroopsToCountry(Country country, int troops)
        {
            if (countries.Contains(country))
            {
                country.AddArmies(troops);
                armies -= troops;
            }
            else
            {
                Debug.Log("This country does not belong to the player");
            }
        }

        public void ResetExchangeCounter()
        {
            exchangeCounter = 1;
        }
    }

    public class StageManager
    {
        List<Player> playerList;
        public Player currentTurnPlayer;
        Map map;

        public StageManager(int numberOfPlayers, Map map) // Initial Stage
        {
            this.map = map;
            if(numberOfPlayers > 4 || numberOfPlayers < 1) // NUMBER OF PLAYERS SHOULD NOT EXCEED 4
            {
                Debug.Log("INVALID VALUES");
            }
            else
            {
                playerList = new List<Player>(); // Initialize player list

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    // Make list of countries for player and add starting country
                    List<Country> newPlayerCountryList = new List<Country>();
                    Country startingCountry = map.GetStartingCountry();
                    startingCountry.AddArmies(1);
                    newPlayerCountryList.Add(startingCountry);

                    // Add player to list
                    Player player = new Player(newPlayerCountryList, i);
                    playerList.Add(player);
                    startingCountry.SetPlayer(player.GetPlayerSlot()); //Set country to playerslot
                }

                while (map.CheckForUnoccupiedCountries()) // Keep adding countries to players until no countries unoccupied
                {
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        if (map.CheckForUnoccupiedCountries())
                        {
                            Country country = map.GetStartingCountry();
                            country.AddArmies(1);
                            playerList[i].AddCountry(country);
                            Debug.Log(country.GetPlayer());
                        }
                    }
                }

                
                for (int i = 0; i < playerList.Count; i++) // keep adding troops to players until 35
                {
                    Player player = playerList[i];

                    while (player.GetTotalArmies() < 35)
                    {
                        player.AddTroopsToCountry(player.GetRandomCountry(), 1);
                    }
                    Debug.Log(player.GetTotalArmies());
                }


                
                currentTurnPlayer = playerList[0]; //Set the turn to player 1
            }
        }

        public void ReinforcingStageStart()
        {
            // Give player initial armies for turn
            int armiesFromCountry; // countries /3 rounded down
            armiesFromCountry = currentTurnPlayer.GetCountryCount();
            armiesFromCountry = (int)Mathf.Floor(armiesFromCountry / 3);

            int armiesFromContinent = map.GetControlledContinents(currentTurnPlayer); // controlled continents
            
            int initialArmies = armiesFromCountry + armiesFromContinent; // Total

            if (initialArmies < 3) initialArmies = 3; // minimum 3

            currentTurnPlayer.SetArmies(initialArmies);
        }

        public void CheckCards()
        {
            currentTurnPlayer.ResetExchangeCounter();
            if (currentTurnPlayer.CardCount() >= 5) currentTurnPlayer.ExchangeOneOfEachCard();
        }
        public void ReinforcingStageExchangeCards()
        {
            Vector3 playerCards = currentTurnPlayer.CardsAsVector();
            while(playerCards.x >= 3)
            {
                playerCards.x = currentTurnPlayer.ExchangeCards(Card.Infantry);
            }
            while(playerCards.y >= 3)
            {
                playerCards.y = currentTurnPlayer.ExchangeCards(Card.Artillery);
            }
            while(playerCards.z >= 3)
            {
                playerCards.z = currentTurnPlayer.ExchangeCards(Card.Cavalry);
            }
            
        }


        public void ReinforcingStageReinforceArmies(Country country, int amount)
        {
            currentTurnPlayer.AddTroopsToCountry(country, amount); 
        }

        public int AttackingStageAttack(Country attacker, Country target, int dice)
        {
            int diceUsed = 0;
            while(attacker.Armies() <= 0 || target.Armies() <= 0)
            {
                diceUsed = 0;
                int attackerRoll = 0;
                for (int i = 0; i < 3; i++) // Roll up to 3 dice for every army
                {
                    if (i < attacker.Armies()) break;
                    attackerRoll += Random.Range(1, 7);
                    diceUsed += 1;
                }
                int targetRoll = 0;
                for(int i = 0; i < 2; i++) // Roll up to 2 dice for every army
                {
                    if (i < target.Armies()) break;
                    targetRoll += Random.Range(1, 7);
                }
                if(attackerRoll > targetRoll) // If attacker rolls higher target loses one army
                {
                    target.SubtractArmies(1);
                }
                if (attackerRoll <= targetRoll) // If attacker rolls less or equal to target they lose one army
                {
                    attacker.SubtractArmies(1);
                }
            }
            if (target.Armies() == 0) target.SetPlayer(currentTurnPlayer.GetPlayerSlot());

            return diceUsed; // To determine the minimum amount of armies the player must deploy
        }

        public int AttackingStageBlitz(Country attacker, Country target, int dice)
        {
            int diceUsed = 0;
            while (attacker.Armies() > 1 && target.Armies() > 0)
            {
                diceUsed = 0;
                int attackerRoll = 0;
                for (int i = 0; i < 3; i++) // Roll up to 3 dice for every army
                {
                    if (i > attacker.Armies()-2) break;
                    int roll = Random.Range(1, 7);
                    if(attackerRoll < roll) attackerRoll = roll;
                    diceUsed += 1;
                    Debug.Log(attackerRoll);
                }
                int targetRoll = 0;
                for (int i = 0; i < 2; i++) // Roll up to 2 dice for every army
                {
                    if (i > target.Armies()-1) break;
                    int roll = Random.Range(1, 7);
                    if (targetRoll < roll) targetRoll = roll;
                    Debug.Log(targetRoll);
                }
                if (attackerRoll > targetRoll) // If attacker rolls higher target loses one army
                {
                    target.SubtractArmies(1);
                    Debug.Log("attacker won");
                }
                if (attackerRoll <= targetRoll) // If attacker rolls less or equal to target they lose one army
                {
                    attacker.SubtractArmies(1);
                    Debug.Log("defender won");
                }
            }
            if (target.Armies() == 0) {

                target.SetPlayer(currentTurnPlayer.GetPlayerSlot());
                GetPlayer(target.GetPlayer()).RemoveCountry(target);
                currentTurnPlayer.AddCountry(target);
            }

            else diceUsed = -1; // To tell if attacker lost;
            return diceUsed; // To determine the minimum amount of armies the player must deploy
        }

        public void FortifyingStage(Country countryDonor, Country countryReciever, int fortificationAmount) // Need to run checks before
        {
            countryDonor.SubtractArmies(fortificationAmount);
            countryReciever.AddArmies(fortificationAmount);
        }

        public void ProgressTurns()
        {
            int index = playerList.IndexOf(currentTurnPlayer);
            index++;
            if (index == playerList.Count) // Loop back to the beginning after the last player
            {
                index = 0;
            }
            currentTurnPlayer = playerList[index];
        }

        public Player GetPlayer(PlayerSlot slot)
        {
            switch (slot)
            {
                case PlayerSlot.Player1: return playerList[0];
                case PlayerSlot.Player2: return playerList[1];
                case PlayerSlot.Player3: return playerList[2];
                case PlayerSlot.Player4: return playerList[3];
                default:
                    Debug.Log("Player lot not in range");
                    return null;
            }
        }

        public bool CheckPlayersAlive()
        {
            int alivePlayers = 0;
            foreach(Player player in playerList)
            {
                if (player.GetCountryCount() > 0)
                {
                    alivePlayers++;
                }
            }

            return alivePlayers > 1;
        }

        public bool CheckPlayerStatus()
        {
            return currentTurnPlayer.GetCountryCount() > 0;
        }

        public void FixPlayerCountryList(Map map)
        {
            foreach(Player player in playerList)
            {
                player.countries.Clear();
                foreach(Country country in map.countries)
                {
                    if(country.GetPlayer() == currentTurnPlayer.GetPlayerSlot())
                    {
                        player.countries.Add(country);
                    }
                }
            }
            map.UpdateCountryLabels();
            map.ShowContinentPlayerControl();
        }

        public Player FindLastPlayer()
        {
            foreach (Player player in playerList)
            {
                if(player.countries.Count == 26)
                {
                    return player;
                }
            }
            return null;
        }
    }
}