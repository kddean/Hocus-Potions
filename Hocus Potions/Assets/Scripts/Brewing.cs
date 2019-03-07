using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brewing {

    class attList {
        public int count;
        public Ingredient.Attributes attribute;

        public attList(int c, Ingredient.Attributes a) {
            count = c;
            attribute = a;
        }
    }

    class modList {
        public int count;
        public Ingredient.Modifiers attribute;

        public modList(int c, Ingredient.Modifiers a) {
            count = c;
            attribute = a;
        }
    }

    public Potion Brew(Ingredient first, Ingredient second, Ingredient third) {
        ResourceLoader rl = GameObject.FindObjectOfType<ResourceLoader>();
        BookManager bm = GameObject.FindObjectOfType<BookManager>();
        bool potionTrue;
        //assign ingredients based on name

        List<attList> attributes = new List<attList>();
        List<modList> modifiers = new List<modList>();
        Ingredient.Attributes[] attArray = new Ingredient.Attributes[first.attributeList.Length + second.attributeList.Length + third.attributeList.Length];
        first.attributeList.CopyTo(attArray, 0);
        second.attributeList.CopyTo(attArray, first.attributeList.Length);
        third.attributeList.CopyTo(attArray, first.attributeList.Length + second.attributeList.Length);

        Ingredient.Attributes? primary = null;
        Ingredient.Attributes? secondary = null;
        Ingredient.Modifiers? mod = null;
        int duration = 0;
        //Sprite image;
        string name = "";
        string image;
        int brewingTime;
        int aCount;

        bool add;
        for (int i = 0; i < attArray.Length; i++) {
            add = true;
            if (System.Enum.IsDefined(typeof(Ingredient.Modifiers), attArray[i].ToString())) {
                Ingredient.Modifiers temp = (Ingredient.Modifiers)System.Enum.Parse(typeof(Ingredient.Modifiers), attArray[i].ToString());
                foreach (modList m in modifiers) {
                    if (m.attribute == temp) {
                        m.count++;
                        add = false;
                        break;
                    }
                }
                if (add) {
                    modifiers.Add(new modList(1, temp));
                }
            } else {
                Ingredient.Attributes temp = attArray[i];
                foreach (attList a in attributes) {
                    if (a.attribute == temp) {
                        a.count++;
                        add = false;
                        break;
                    }
                }
                if (add) {
                    attributes.Add(new attList(1, temp));
                }
            }
        }

        List<attList> sortedAtt = attributes.OrderByDescending(attList => attList.count).ToList();
        List<modList> sortedMod = modifiers.OrderByDescending(modList => modList.count).ToList();

        aCount = sortedAtt.Count();
        if (sortedAtt[0].count >= 3) {              //fails if no attribute exists at least 3 times
                                                    //accounts for ties in primary attribute
            if (aCount > 1 && sortedAtt[0].count == sortedAtt[1].count) {
                if (aCount > 2 && sortedAtt[1].count == sortedAtt[2].count) {  //three-way tie
                    int flip1 = Random.Range(0, 3);
                    int flip2 = Random.Range(0, 2);
                    switch (flip1) {
                        case 0:
                            primary = sortedAtt[0].attribute;
                            if (flip2 == 0) {
                                secondary = sortedAtt[1].attribute;
                            } else {
                                secondary = sortedAtt[2].attribute;
                            }
                            break;
                        case 1:
                            primary = sortedAtt[1].attribute;
                            if (flip2 == 0) {
                                secondary = sortedAtt[0].attribute;
                            } else {
                                secondary = sortedAtt[2].attribute;
                            }
                            break;
                        case 2:
                            primary = sortedAtt[2].attribute;
                            if (flip2 == 0) {
                                secondary = sortedAtt[0].attribute;
                            } else {
                                secondary = sortedAtt[1].attribute;
                            }
                            break;
                        default:
                            break;
                    }
                }
                int flip = Random.Range(0, 2);           //two-way tie
                if (flip == 0) {
                    primary = sortedAtt[0].attribute;
                    secondary = sortedAtt[1].attribute;
                } else {
                    primary = sortedAtt[1].attribute;
                    secondary = sortedAtt[0].attribute;
                }
            } else {
                primary = sortedAtt[0].attribute;       //no tie

                if (aCount == 1) {
                    secondary = null;
                } else {
                    if (sortedAtt[1].count >= 2) {
                        if (aCount > 2 && sortedAtt[1].count == sortedAtt[2].count) {
                            if (aCount > 3 && sortedAtt[2].count == sortedAtt[3].count) {    //three-way tie
                                int flip = Random.Range(0, 3);
                                switch (flip) {
                                    case 0:
                                        secondary = sortedAtt[1].attribute;
                                        break;
                                    case 1:
                                        secondary = sortedAtt[2].attribute;
                                        break;
                                    case 2:
                                        secondary = sortedAtt[3].attribute;
                                        break;
                                    default:
                                        Debug.Log("how is this possible");
                                        break;
                                }
                            } else {
                                int flip = Random.Range(0, 2);          //two-way tie
                                if (flip == 0) {
                                    secondary = sortedAtt[1].attribute;
                                } else {
                                    secondary = sortedAtt[2].attribute;
                                }
                            }
                        } else {
                            secondary = sortedAtt[1].attribute;         //no tie
                        }
                        //Secondary is null if no other attribute appears 2 or more times
                    } else {
                        secondary = null;
                    }
                }
            }
            //Modifiers - *should be functional but this will need some major tweaking in the long run*
            //Combinations of 2 mugwort + 1 poppy or 1 nightshade end up as transformation secondaries with no animal modifier 
            if (sortedMod.Count() < 1) {
                mod = null;
            } else {
                if (primary == Ingredient.Attributes.transformation || secondary == Ingredient.Attributes.transformation) {
                    foreach (modList m in sortedMod) {
                        if (m.attribute == Ingredient.Modifiers.cat || m.attribute == Ingredient.Modifiers.sheep || m.attribute == Ingredient.Modifiers.chicken) {
                            mod = m.attribute;
                            break;
                        }
                    }
                } else {
                    foreach (modList m in sortedMod) {
                        if (m.attribute == Ingredient.Modifiers.magicMM || m.attribute == Ingredient.Modifiers.magicPP) {
                            mod = m.attribute;
                            break;
                        }
                    }
                }
            }

            //TODO: finalize naming conventions - Currently just named by primary attrbute
            //Potion name generation
            if (primary.ToString().Contains("dye")) {
                name = primary.ToString().Substring(3) + " dye";
            } else {
                name = primary.ToString() + " Potion";
                name = char.ToUpper(name[0]) + name.Substring(1);
            }

            //TODO: Might need this later but for now it's ignoring modifiers
            /*switch (mod) {
                case Ingredient.Modifiers.magicPP:
                    name = "Super " + name;
                    break;
                case Ingredient.Modifiers.magicMM:
                    name = "Weak " + name;
                    break;
                case Ingredient.Modifiers.cat:
                    name = "Cat " + name;
                    break;
                case Ingredient.Modifiers.sheep:
                    name = "Sheep " + name;
                    break;
                case Ingredient.Modifiers.chicken:
                    name = "Chicken " + name;
                    break;
                default:
                    break;
            }*/

            //Assigning sprites, effect duration, and brewing time
            switch (primary) {
                case Ingredient.Attributes.healing:
                    duration = 10;
                    brewingTime = 50;
                    image = "Potions/potions_healing";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.healing)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.healing);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.healing)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.healing);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.healing)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.healing);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if(bm.potionDiscovery.TryGetValue("Healing", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Healing"] = potionTrue;
                    }

                    break;
                case Ingredient.Attributes.sleep:
                    duration = 120;
                    brewingTime = 60;
                    image = "Potions/potions_sleep";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.sleep)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.sleep);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.sleep)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.sleep);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.sleep)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.sleep);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if(bm.potionDiscovery.TryGetValue("Sleeping", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Sleeping"] = potionTrue;
                    }    
                            break;
                case Ingredient.Attributes.invisibility:
                    duration = 100;
                    brewingTime = 90;
                    image = "Potions/potions_invisibility";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.invisibility)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.invisibility);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.invisibility)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.invisibility);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.invisibility)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.invisibility);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if(bm.potionDiscovery.TryGetValue("Invisibility", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Invisibility"] = potionTrue;
                    }

                    break;
                case Ingredient.Attributes.poison:
                    duration = 100;
                    brewingTime = 60;
                    image = "Potions/potions_poison";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.poison)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.poison);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.poison)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.poison);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.poison)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.poison);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if (bm.potionDiscovery.TryGetValue("Poison", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Poison"] = potionTrue;
                    }

                    break;
                case Ingredient.Attributes.transformation:
                    duration = 100;
                    brewingTime = 90;
                    image = "Potions/potions_transform";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.transformation)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.transformation);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.transformation)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.transformation);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.transformation)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.transformation);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if (bm.potionDiscovery.TryGetValue("Transformation", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Transformation"] = potionTrue;
                    }
                    break;
                case Ingredient.Attributes.mana:
                    duration = 10;
                    brewingTime = 60;
                    image = "Potions/potions_mana";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.mana)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.mana);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.mana)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.mana);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.mana)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.mana);
                    }

                    if (bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if(bm.potionDiscovery.TryGetValue("Mana", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Mana"] = potionTrue;
                    }
                    break;
                case Ingredient.Attributes.speed:
                    duration = 100;
                    brewingTime = 80;
                    image = "Potions/potions_speed";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.speed)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.speed);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.speed)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.speed);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.speed)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.speed);
                    }

                    if(bm.potionDiscovery == null)
                    {
                        break;
                    }
                    else if (bm.potionDiscovery.TryGetValue("Speed", out potionTrue))
                    {
                        potionTrue = true;
                        bm.potionDiscovery["Speed"] = potionTrue;
                    }
                    break;
                case Ingredient.Attributes.dyeBlack:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_black";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeBlack)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeBlack);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeBlack)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeBlack);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeBlack)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeBlack);
                    }
                    break;
                case Ingredient.Attributes.dyeBlue:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_blue";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeBlue)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeBlue);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeBlue)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeBlue);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeBlue)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeBlue);
                    }
                    break;
                case Ingredient.Attributes.dyeGreen:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_green";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeGreen)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeGreen);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeGreen)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeGreen);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeGreen)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeGreen);
                    }
                    break;
                case Ingredient.Attributes.dyePurple:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_purple";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyePurple)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyePurple);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyePurple)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyePurple);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyePurple)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyePurple);
                    }
                    break;
                case Ingredient.Attributes.dyeRed:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_red";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeRed)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeRed);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeRed)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeRed);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeRed)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeRed);
                    }
                    break;
                case Ingredient.Attributes.dyeWhite:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_white";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeWhite)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeWhite);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeWhite)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeWhite);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeWhite)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeWhite);
                    }
                    break;
                case Ingredient.Attributes.dyeYellow:
                    duration = 0;
                    brewingTime = 80;
                    image = "Potions/dye_yellow";
                    if (!rl.knownAttributes[first].Contains(Ingredient.Attributes.dyeYellow)) {
                        rl.knownAttributes[first].Add(Ingredient.Attributes.dyeYellow);
                    }
                    if (!rl.knownAttributes[second].Contains(Ingredient.Attributes.dyeYellow)) {
                        rl.knownAttributes[second].Add(Ingredient.Attributes.dyeYellow);
                    }
                    if (!rl.knownAttributes[third].Contains(Ingredient.Attributes.dyeYellow)) {
                        rl.knownAttributes[third].Add(Ingredient.Attributes.dyeYellow);
                    }
                    break;
                default:
                    name = "Failed potion";
                    duration = 0;
                    brewingTime = 30;
                    image = "Potions/potions_null";
                    break;
            }

            return new Potion(name, image, duration, primary, secondary, mod, brewingTime);

        } else {
            //for failed potions TO DO: clean up a bit later
            name = "Failed potion";
            primary = null;
            secondary = null;
            mod = null;
            duration = 0;
            brewingTime = 30;
            image = "Potions/potions_null";
            if (bm.potionDiscovery == null)
            {
               
            }
            else if (bm.potionDiscovery.TryGetValue("Odd", out potionTrue))
            {
                potionTrue = true;
                bm.potionDiscovery["Odd"] = potionTrue;
            }
            Potion pot = new Potion(name, image, duration, primary, secondary, mod, brewingTime);
            return pot;
        }
    }

}
