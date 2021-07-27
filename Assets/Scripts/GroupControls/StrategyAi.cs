using UnityEngine;

public class StrategyAi : MonoBehaviour
{
    //scroll oldalra, határokkal
    //egység születése után döntés, hogy milyen role-ban legyen
    //folyamatos csekkolása annak, hogy kell-e role-t cserélni

    //külön művelet a
        //születések szabályzására
        //cserére
        //scoutingra, ki merre menjen
            //de a támadás, védekezés, menekülés, az nem ilyen szinten lesz eldöntve


    /*
    1. hogy kellene megoldani, hogy ne minden bogár csinálja ugyanazt?
     -> azaz mindegyik azt csinálja, amit neki épp kell?
        adott a térkép, amit részekre kell szedni, és azon belül értelmezhetőek a döntések?

    2. hogy lesz összekötve a stratégia AI és a taktika AI?

    3. azt hogy lehet megoldani, hogy a megfelelő szintű bogarakat gyártsa az AI?


    születéskor súlyozni a bug stratégiát
    Paraméter a saját és ellenség power
    Utolsó 3 paraméter megjegyzése
    Def a bázishoz közel
    Off a várostól tavolabb
    Scoutt még távolabb

    az egyes bug-ok típusát menet közben is lehet cserélni

    */

    void Update()
    {
        
    }
}
