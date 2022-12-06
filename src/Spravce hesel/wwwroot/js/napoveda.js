let ROUTE = document.getElementById("ROUTE").innerHTML.split("-");

function load(sekce) {
    $.getJSON("../json/napoveda.json", function (napoveda) {
        const prehravac_obsah = document.getElementById("napoveda_prehravac_obsah");
        const prehravac_nadpis = document.getElementById("napoveda_prehravac_nadpis")
        let cesta = "";

        let aktualniNapoveda = napoveda;

        try {
            sekce.forEach(rt => {
                aktualniNapoveda = aktualniNapoveda[rt];
                document.getElementById("napoveda_navigace_menu_cesta").innerHTML += aktualniNapoveda["Nazev"];
                if (rt != sekce[sekce.length - 1])
                    document.getElementById("napoveda_navigace_menu_cesta").innerHTML += " > ";
            })

            prehravac_nadpis.innerHTML = aktualniNapoveda["Nazev"];
            prehravac_obsah.innerHTML = aktualniNapoveda["Obsah"];
        }

        catch (chyba) {
            prehravac_nadpis.innerHTML = "Došlo k chybě!";
            prehravac_obsah.innerHTML = "<i>Obsah této nápovědy není nyní přístupný!</i>";
        }

        //ROUTE.forEach(RT => {
        //    document.getElementById("napoveda_navigace_menu_cesta").innerHTML += RT;
        //    if (RT != ROUTE[ROUTE.length - 1])
        //        document.getElementById("napoveda_navigace_menu_cesta").innerHTML += " > ";
        //})
    })
}

function zpet() {
    if (ROUTE.length != 1) {
        ROUTE.pop();
        reload(ROUTE);
    }
}

function rozbalitMenu(tlacitko) {
    if (document.getElementById("napoveda_menu_seznam").className == "skryty") {
        document.getElementById("napoveda_menu_seznam").className = "";
        tlacitko.innerHTML = "▲";
    }
    else {
        document.getElementById("napoveda_menu_seznam").className = "skryty";
        tlacitko.innerHTML = "▼";
    }
}

load(ROUTE);
