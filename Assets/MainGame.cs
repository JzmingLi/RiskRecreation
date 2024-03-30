using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiskRecreation;
using JetBrains.Annotations;
using System;
using TMPro;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Reflection;

public class MainGame : MonoBehaviour
{
    // Prefabs to instantiate
    public GameObject text;
    public GameObject playerSelectionVisuals;
    public GameObject reinforcingStageHUD;
    public int playerCount;
    public int reinforcements;
    public GameObject reinforceButton;
    public Country reinforceCountry;
    public bool reinforced;
    public bool exchanged;
    public GameObject attackerButton; //reused for fortification stage
    public GameObject targetButton;
    public Country attackingCountry;
    public Country defendingCountry;
    public bool attackerChosen; //reused for fortification stage
    public bool defenderChosen;
    public bool attacking;
    public bool incrementUp;
    public bool incrementDown;
    public bool fortified;
    public GameObject valuePicker;
    public bool skipAndFortify;
    public GameObject fortifyButton;


    void DestroyAllChildren(GameObject target)
    {
        foreach (Transform child in target.transform)
        {
            DestroyAllChildren(child.gameObject);
        }
        Destroy(target);
    }

    void CheckAndMakeConnections(Country country, List<Country> countryList) // Method containing switch statement for connections
    {
        string countryName = country.Name;
        switch (countryName)
        {
            // Aerthos
            case "Solara":
                country.MakeConnection(FindCountry(countryList, "Verdantia"));
                country.MakeConnection(FindCountry(countryList, "Lumina"));
                break;
            case "Verdanita":
                country.MakeConnection(FindCountry(countryList, "Lumina"));
                country.MakeConnection(FindCountry(countryList, "Terranova"));
                break;
            case "Lumina":
                country.MakeConnection(FindCountry(countryList, "Celestia"));
                country.MakeConnection(FindCountry(countryList, "Terranova"));
                country.MakeConnection(FindCountry(countryList, "Verdantia"));
                break;
            case "Celestia":
                country.MakeConnection(FindCountry(countryList, "Terranova"));
                country.MakeConnection(FindCountry(countryList, "Sylvaria"));
                break;
            case "Terranova":
                country.MakeConnection(FindCountry(countryList, "Sylvaria"));
                country.MakeConnection(FindCountry(countryList, "Verdantia"));
                break;
            case "Sylvaria":
                country.MakeConnection(FindCountry(countryList, "Mythos"));
                country.MakeConnection(FindCountry(countryList, "Helios"));
                break;
            // Novaris
            case "Mythos":
                country.MakeConnection(FindCountry(countryList, "Meridian"));
                country.MakeConnection(FindCountry(countryList, "Arcadia"));
                country.MakeConnection(FindCountry(countryList, "Aurelia"));
                country.MakeConnection(FindCountry(countryList, "Crystallia"));
                break;
            case "Meridian":
                country.MakeConnection(FindCountry(countryList, "Arcadia"));
                country.MakeConnection(FindCountry(countryList, "Valoria"));
                break;
            case "Arcadia":
                country.MakeConnection(FindCountry(countryList, "Valoria"));
                country.MakeConnection(FindCountry(countryList, "Solstice"));
                country.MakeConnection(FindCountry(countryList, "Aurelia"));
                break;
            case "Aurelia":
                country.MakeConnection(FindCountry(countryList, "Solstice"));
                break;
            case "Solstice":
                country.MakeConnection(FindCountry(countryList, "Valoria"));
                break;
            // Zephyria
            case "Crystallia":
                country.MakeConnection(FindCountry(countryList, "Helios"));
                country.MakeConnection(FindCountry(countryList, "Aquatia"));
                country.MakeConnection(FindCountry(countryList, "Chronos"));
                break;
            case "Chronos":
                country.MakeConnection(FindCountry(countryList, "Aetheria"));
                break;
            case "Aetheria":
                country.MakeConnection(FindCountry(countryList, "Emberia"));
                break;
            case "Emberia":
                country.MakeConnection(FindCountry(countryList, "Nocturnia"));
                country.MakeConnection(FindCountry(countryList, "Aquatia"));
                break;
            case "Aquatia":
                country.MakeConnection(FindCountry(countryList, "Nocturnia"));
                break;
            // Eldoria
            case "Helios":
                country.MakeConnection(FindCountry(countryList, "Seraphia"));
                country.MakeConnection(FindCountry(countryList, "Umbra"));
                break;
            case "Umbra":
                country.MakeConnection(FindCountry(countryList, "Tempestia"));
                country.MakeConnection(FindCountry(countryList, "Seraphia"));
                break;
            case "Tempestia":
                country.MakeConnection(FindCountry(countryList, "Solaris"));
                country.MakeConnection(FindCountry(countryList, "Vortexia"));
                country.MakeConnection(FindCountry(countryList, "Seraphia"));
                break;
            case "Solaris":
                country.MakeConnection(FindCountry(countryList, "Elysium"));
                country.MakeConnection(FindCountry(countryList, "Vortexia"));
                country.MakeConnection(FindCountry(countryList, "Nexus"));
                break;
            case "Elysium":
                country.MakeConnection(FindCountry(countryList, "Nexus"));
                break;
            case "Vortexia":
                country.MakeConnection(FindCountry(countryList, "Nexus"));
                country.MakeConnection(FindCountry(countryList, "Seraphia"));
                break;
            default:
                Debug.Log("Connections Finished!");
                break;
        }
    }

    void Start()
    {
        StartCoroutine(Main());
    }

    // Method to help find country for linking
    Country FindCountry(List<Country> countryList, string name)
    {
        foreach (Country country in countryList)
        {
            if (country.Name == name) { return country; }
        }
        Debug.Log("Country with name " + name + " doesn't exist!");
        return null;
    }

    IEnumerator Main()
    {
        /* The map is already set up in Unity, but to showcase the DrawMap method that initailizes the map using code,
         * We gathered all the object's positions and sprites and fed the information to the Map class to redraw them map
         * using the method we created, and then deleted the ones made in unity.
         * UPDATE: Draw map may not be necessary because to clone GameObjects we have to initialize them anyway
         * The other way we could do it is assigning prefabs in serialized fields but thats a Unity only thing so I don't think its worth it
         */
        
        //Getting Continents by getting all with tag
        GameObject[] continentGameObjects = GameObject.FindGameObjectsWithTag("Continent");

        //Getting Countries by getting all children of each continent
        List<List<GameObject>> countryGameObjects = new List<List<GameObject>>();
        foreach (GameObject continent in continentGameObjects)
        {
            List<GameObject> continentCountries = new List<GameObject>();
            foreach (Transform child in continent.transform)
            {
                continentCountries.Add(child.gameObject);
            }
            countryGameObjects.Add(continentCountries);
        }

        // These lists will be passed onto the map class later
        List<Continent> continentList = new List<Continent>();
        List<Country> countryList = new List<Country>();

        foreach (GameObject continentGameObject in continentGameObjects)
        {
            // Get the list of country gameobjects for the continent
            List<GameObject> countries = countryGameObjects[Array.IndexOf(continentGameObjects, continentGameObject)];
            // Here the original object is cloned and the clone is set as the actual continent object
            // Also the control value is just set to the amount of countries in the continent
            Continent continent = new Continent(continentGameObject.name, countries.Count, Instantiate(continentGameObject, continentGameObject.transform.position, Quaternion.identity));
            // Make the actual country objects (position is required in the constructor but after realizing draw map is redundant it may be useless)
            // Can't hurt to have it though, also add the countries to the country list
            foreach(GameObject country in countries)
            {
                Country newCountry = new Country(country.name, continent, new Vector2(country.transform.position.x, country.transform.position.y), Instantiate(country));
                continent.AddCountry(newCountry);
                countryList.Add(newCountry);
            }
            foreach (Transform child in continent.Sprite.transform)
            {
                // Add nametags for clarity
                GameObject nametag = Instantiate(text, child.position + new Vector3(0,-0.5f,0), child.rotation);
                nametag.GetComponent<TextMeshPro>().text = child.gameObject.name;
                // Destroy the children in the new instance (Noticed this bug after playtesting)
                Destroy(child.gameObject);
            }
            // It shouldn't matter because it passes a reference to the continent to the list, but just in case now we add the continent with all of its countries
            continentList.Add(continent);
        }

        // Delete the original editor made objects
        foreach (GameObject continentGameObject in continentGameObjects)
        {
            Destroy(continentGameObject);
        }

        // Manual Connection Assignment

        foreach(Country country in countryList)
        {
            CheckAndMakeConnections(country, countryList);
        }

        // Add the Map
        Map gameMap = new Map(countryList, continentList);

        // Choose player colours
        gameMap.ChoosePlayerColors(Color.red, Color.blue, Color.yellow, Color.green, Color.gray);

        // Make the map look normal because I made them random colours in the editor originally
        gameMap.UpdateCountryLabels();
        gameMap.ShowContinentPlayerControl();

        // Let player choose number of players, bring up ui and delete it after finished
        GameObject SelectPlayers = Instantiate(playerSelectionVisuals);
        while(playerCount == 0)
        {
            yield return null;
        }
        Debug.Log(playerCount);
        DestroyAllChildren(SelectPlayers);

        // Create stagemanager
        StageManager stageManager = new StageManager(playerCount, gameMap);
        gameMap.UpdateCountryLabels();
        gameMap.ShowContinentPlayerControl();

        while (stageManager.CheckPlayersAlive())
        {
            stageManager.FixPlayerCountryList(gameMap);
            if (stageManager.CheckPlayerStatus())
            {
                
                //Reinforcing Stage 
                stageManager.ReinforcingStageStart();
                ReinforcingStageHUD reinHUD = Instantiate(reinforcingStageHUD).GetComponent<ReinforcingStageHUD>();
                reinHUD.messageBox.text = "";
                Player currentPlayer = stageManager.currentTurnPlayer;
                reinHUD.UpdateVisuals(currentPlayer.GetPlayerSlot(), currentPlayer.CardsAsVector(), currentPlayer.armies, gameMap.PlayerColors[currentPlayer.GetPlayerSlot()]);

                List<Country> playerCountries = currentPlayer.countries;
                List<GameObject> buttons = new List<GameObject>();
                foreach (Country country in playerCountries) //spawn buttons
                {
                    GameObject button = Instantiate(reinforceButton, country.Sprite.transform);
                    button.GetComponent<ReinforceButton>().country = country;
                    buttons.Add(button);
                    Debug.Log(country.Name + country.GetPlayer());
                }

                reinforcements = currentPlayer.armies;
                stageManager.CheckCards();
                while (reinforcements > 0)
                {
                    while (!reinforced && !exchanged)
                    {
                        yield return null;
                    }
                    if (reinforced)
                    {
                        currentPlayer.AddTroopsToCountry(reinforceCountry, 1);
                        gameMap.UpdateCountryLabels();
                        reinHUD.UpdateVisuals(currentPlayer.GetPlayerSlot(), currentPlayer.CardsAsVector(), currentPlayer.armies, gameMap.PlayerColors[currentPlayer.GetPlayerSlot()]);
                    }
                    if (exchanged) //Quick exchange incase running out of time
                    {
                        /**
                        for(int i = 0; i < 100; i++)
                        {
                            currentPlayer.AddCard(Card.Artillery);
                        }
                        **/
                        //Testing^
                        stageManager.ReinforcingStageExchangeCards();
                        reinforcements = currentPlayer.armies;
                        reinHUD.UpdateVisuals(currentPlayer.GetPlayerSlot(), currentPlayer.CardsAsVector(), currentPlayer.armies, gameMap.PlayerColors[currentPlayer.GetPlayerSlot()]);
                    }

                    reinHUD.UpdateVisuals(currentPlayer.GetPlayerSlot(), currentPlayer.CardsAsVector(), currentPlayer.armies, gameMap.PlayerColors[currentPlayer.GetPlayerSlot()]);
                    reinforced = false;
                    exchanged = false;
                    yield return null;
                }

                foreach (GameObject button in buttons)
                {

                    Destroy(button);

                }

                // Attacking Stage

                Vector3 playerCardPos = reinHUD.playerBackground.gameObject.transform.position;
                GameObject playerCard = Instantiate(reinHUD.playerBackground.gameObject, playerCardPos, Quaternion.identity);
                Vector3 messageBoxPos = reinHUD.messageBox.gameObject.transform.position;
                GameObject messageBox = Instantiate(reinHUD.messageBox.gameObject, messageBoxPos, Quaternion.identity);
                DestroyAllChildren(reinHUD.gameObject);

                List<GameObject> attackerButtons = new List<GameObject>();
                foreach (Country country in playerCountries)
                {
                    bool hasHostileNeighbour = false;
                    foreach(Country connection in country.GetConnections())
                    {
                        if (country.GetPlayer() != connection.GetPlayer()) hasHostileNeighbour = true;
                    }
                    if (hasHostileNeighbour)
                    {
                        GameObject button = Instantiate(attackerButton, country.Sprite.transform);
                        button.GetComponent<AttackingButton>().country = country;
                        attackerButtons.Add(button);
                    }
                    
                }

                messageBox.GetComponent<TextMeshPro>().text = "Choose a Country as Attacker";

                bool canFortify = false;
                foreach(Country country in currentPlayer.countries)
                {
                    foreach (Country connection in country.GetConnections())
                    {
                        if (connection.GetPlayer() == country.GetPlayer()) canFortify = true;
                    }
                }
                GameObject fButton = Instantiate(fortifyButton);
                if (!canFortify) fButton.SetActive(false);
                while (!attackerChosen && !skipAndFortify)
                {
                    yield return null;
                }
                if (attackerChosen)
                {
                    attackerChosen = false;
                    DestroyAllChildren(fButton);

                    foreach (GameObject button in attackerButtons)
                    {

                        Destroy(button);
                    }

                    List<Country> targets = new List<Country>();
                    foreach (Country connection in attackingCountry.GetConnections())
                    {
                        if (connection.GetPlayer() != attackingCountry.GetPlayer())
                        {
                            targets.Add(connection);
                        }
                    }

                    List<GameObject> defenderButtons = new List<GameObject>();
                    foreach (Country country in targets)
                    {
                        GameObject button = Instantiate(targetButton, country.Sprite.transform);
                        button.GetComponent<TargetButton>().country = country;
                        defenderButtons.Add(button);
                    }

                    messageBox.GetComponent<TextMeshPro>().text = "Choose a Country as a Target";

                    while (!defenderChosen)
                    {
                        yield return null;
                    }
                    defenderChosen = false;

                    foreach (GameObject button in defenderButtons)
                    {
                        Destroy(button);
                    }

                    //Blitz in case no time to implement normal attacks
                    int diceUsed = stageManager.AttackingStageBlitz(attackingCountry, defendingCountry, 3);
                    Debug.Log(diceUsed);
                    gameMap.UpdateCountryLabels();

                    if (diceUsed == -1) // Attacker loss output value
                    {
                        messageBox.GetComponent<TextMeshPro>().text = "Blitz! You lost! Your turn ends!";

                        yield return new WaitForSeconds(2);
                        
                    }
                    else
                    {
                        messageBox.GetComponent<TextMeshPro>().text = "Blitz! You won! Deploy troops to your new Country!";
                        GameObject vp = Instantiate(valuePicker);
                        List<int> possibleValues = new List<int>();
                        for (int i = diceUsed; i < attackingCountry.Armies(); i++)
                        {
                            possibleValues.Add(i);
                        }
                        int currentIndex = 0;
                        TextMeshPro valueVisual = vp.GetComponentInChildren<TextMeshPro>();
                        valueVisual.text = possibleValues[currentIndex].ToString();

                        while (!fortified)
                        {
                            while (!incrementDown && !incrementUp && !fortified)
                            {
                                yield return null;
                            }
                            if (incrementDown)
                            {
                                incrementDown = false;
                                if (currentIndex != 0) currentIndex--;
                            }
                            if (incrementUp)
                            {
                                incrementUp = false;
                                if (currentIndex != possibleValues.Count - 1) currentIndex++;
                            }
                            valueVisual.text = possibleValues[currentIndex].ToString();
                            yield return null;
                        }
                        fortified = false;

                        DestroyAllChildren(vp);
                        Debug.Log(defendingCountry.GetPlayer());
                        attackingCountry.TransferArmies(possibleValues[currentIndex], defendingCountry);
                        gameMap.UpdateCountryLabels();
                        gameMap.ShowContinentPlayerControl();
                    }
                    currentPlayer.AddCard();
                    DestroyAllChildren(messageBox);
                    DestroyAllChildren(playerCard);
                }
                else if (skipAndFortify) // Fortify Stage
                {
                    skipAndFortify = false;
                    DestroyAllChildren(fButton);
                    foreach (GameObject button in attackerButtons)
                    {

                        Destroy(button);
                    }

                    messageBox.GetComponent<TextMeshPro>().text = "Choose a Country as a Donor";

                    List<GameObject> countryOptions = new List<GameObject>();
                    List<Country> fortificationConnections = new List<Country>();
                    foreach (Country country in playerCountries)
                    {
                        bool hasFriendlyOnBorder = false;
                        foreach (Country connection in country.GetConnections())
                        {
                            if (connection.GetPlayer() == country.GetPlayer()) hasFriendlyOnBorder = true;
                        }
                        if (hasFriendlyOnBorder) fortificationConnections.Add(country);
                    }

                    foreach (Country country in fortificationConnections)
                    {
                        GameObject button = Instantiate(attackerButton, country.Sprite.transform);
                        button.GetComponent<AttackingButton>().country = country;
                        countryOptions.Add(button);
                    }

                    while (!attackerChosen)
                    {
                        yield return null;
                    }
                    attackerChosen = false;


                    foreach (GameObject button in countryOptions)
                    {

                        Destroy(button);
                    }

                    List<Country> fortTargets = new List<Country>();
                    foreach (Country country in attackingCountry.GetConnections())
                    {
                        if (country.GetPlayer() == attackingCountry.GetPlayer()) fortTargets.Add(country);
                    }

                    List<GameObject> fortificationButtons = new List<GameObject>();
                    foreach (Country country in fortTargets)
                    {
                        GameObject button = Instantiate(targetButton, country.Sprite.transform);
                        button.GetComponent<TargetButton>().country = country;
                        fortificationButtons.Add(button);
                    }

                    messageBox.GetComponent<TextMeshPro>().text = "Choose a Country as a Target";

                    while (!defenderChosen)
                    {
                        yield return null;
                    }
                    defenderChosen = false;

                    foreach (GameObject button in fortificationButtons)
                    {
                        Destroy(button);
                    }

                    GameObject vp = Instantiate(valuePicker);
                    List<int> possibleValues = new List<int>();
                    for (int i = 1; i < attackingCountry.Armies(); i++)
                    {
                        possibleValues.Add(i);
                    }
                    int currentIndex = 0;
                    TextMeshPro valueVisual = vp.GetComponentInChildren<TextMeshPro>();
                    valueVisual.text = possibleValues[currentIndex].ToString();

                    while (!fortified)
                    {
                        while (!incrementDown && !incrementUp && !fortified)
                        {
                            yield return null;
                        }
                        if (incrementDown)
                        {
                            incrementDown = false;
                            if (currentIndex != 0) currentIndex--;
                        }
                        if (incrementUp)
                        {
                            incrementUp = false;
                            if (currentIndex != possibleValues.Count - 1) currentIndex++;
                        }
                        valueVisual.text = possibleValues[currentIndex].ToString();
                        yield return null;
                    }
                    fortified = false;

                    DestroyAllChildren(vp);

                    attackingCountry.TransferArmies(possibleValues[currentIndex], defendingCountry);
                    gameMap.UpdateCountryLabels();

                    DestroyAllChildren(messageBox);
                    DestroyAllChildren(playerCard);
                }
            }
            
                
            stageManager.ProgressTurns();

            yield return null;
        }


        
        


        yield return 0;
    }
}
