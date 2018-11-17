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

        //assign ingredients based on name

        List<attList> attributes = new List<attList>();
        List<modList> modifiers = new List<modList>();
        Ingredient.Attributes[] attArray = new Ingredient.Attributes[] { first.attributeList[0], first.attributeList[1], first.attributeList[2],
                                                                         second.attributeList[0], second.attributeList[1], second.attributeList[2],
                                                                         third.attributeList[0], third.attributeList[1], third.attributeList[2]};

        Ingredient.Attributes? primary = null;
        Ingredient.Attributes? secondary = null;
        Ingredient.Modifiers? mod = null;
        int duration = 0;
        //Sprite image;
        string name = "";
        Sprite image;
        int brewingTime;
        int aCount;

        bool add;
        for (int i = 0; i < 9; i++) {
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
            name = primary.ToString() + " Potion";
            name = char.ToUpper(name[0]) + name.Substring(1);

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
                    image = Resources.Load<Sprite>("Potions/potions_healing");
                    break;
                case Ingredient.Attributes.sleep:
                    duration = 100;
                    brewingTime = 60;
                    image = Resources.Load<Sprite>("Potions/potions_sleep");
                    break;
                case Ingredient.Attributes.invisibility:
                    duration = 100;
                    brewingTime = 90;
                    image = Resources.Load<Sprite>("Potions/potions_invisibility");
                    break;
                case Ingredient.Attributes.poison:
                    duration = 100;
                    brewingTime = 60;
                    image = Resources.Load<Sprite>("Potions/potions_poison");
                    break;
                case Ingredient.Attributes.transformation:
                    duration = 100;
                    brewingTime = 90;
                    image = Resources.Load<Sprite>("Potions/potions_transform");
                    break;
                case Ingredient.Attributes.mana:
                    duration = 10;
                    brewingTime = 60;
                    image = Resources.Load<Sprite>("Potions/potions_mana");
                    break;
                case Ingredient.Attributes.speed:
                    duration = 100;
                    brewingTime = 80;
                    image = Resources.Load<Sprite>("Potions/potions_speed");
                    break;
                default:
                    name = "Failed potion";
                    duration = 0;
                    brewingTime = 30;
                    image = Resources.Load<Sprite>("Potions/potions_null");
                    break;
            }

            Potion pot = new Potion(name, image, duration, primary, secondary, mod, brewingTime);
            return pot;

        } else {
            //for failed potions TO DO: clean up a bit later
            name = "Failed potion";
            primary = null;
            secondary = null;
            mod = null;
            duration = 0;
            brewingTime = 30;
            image = Resources.Load<Sprite>("Potions/potions_null");
            Potion pot = new Potion(name, image, duration, primary, secondary, mod, brewingTime);
            return pot;
        }
    }

}
