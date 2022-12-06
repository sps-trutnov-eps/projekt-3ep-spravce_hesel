let ROUTE = document.getElementById("ROUTE").innerHTML.split("-");

function reload(sekce) {
    $.getJSON("../json/napoveda.json", function (napoveda) {

        const prehravac_obsah = document.getElementById("napoveda_prehravac_obsah");
        const prehravac_nadpis = document.getElementById("napoveda_prehravac_nadpis")

        let aktualniNapoveda = napoveda;

        try {
            sekce.forEach(rt => {
                aktualniNapoveda = aktualniNapoveda[rt];
            })

            prehravac_nadpis.innerHTML = aktualniNapoveda["Nazev"];
            prehravac_obsah.innerHTML = aktualniNapoveda["Obsah"];
        }

        catch {
            prehravac_nadpis.innerHTML = "Došlo k chybě!";
            prehravac_obsah.innerHTML = "Obsah této nápovědy není nyní přístupný!";
        }

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

reload(ROUTE);
