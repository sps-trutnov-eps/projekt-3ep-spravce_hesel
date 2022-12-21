function ziskatNavigaci(napoveda, navigace = false, adresa = null) {
    let html = "";
    let podsekce = Object.keys(napoveda);
    if (podsekce.length > 0 && podsekce[0] == "&Nazev") {
        podsekce.shift();
    }

    if (podsekce.length != 0) {
        if (navigace)
            html += '<ul id="napoveda_menu_seznam" class="skryty">';
        else
            html += "<ul>";
        
        podsekce.forEach(sekce => {
            if (adresa == null) {
                html += '<li><a href="/Napoveda/' + sekce + '">' + napoveda[sekce]["&Nazev"];
                html += ziskatNavigaci(napoveda[sekce], false, sekce);
            } else {
                let aktualniAdresa = adresa + "-" + [sekce];
                html += '<li><a href="/Napoveda/' + aktualniAdresa + '">' + napoveda[sekce]["&Nazev"];
                html += ziskatNavigaci(napoveda[sekce],false, aktualniAdresa);
            }
            html += "</a></li>";
        });
        html += "</ul>";
    }

    return html;
}

function ziskatSouvisejici(napoveda, adresa) {
    let html = "";

    let castecnaAdresa = adresa;
    let cil = castecnaAdresa.pop();
    let aktualniNapoveda = napoveda;

    castecnaAdresa.forEach(sekce => {
        aktualniNapoveda = aktualniNapoveda[sekce];
    });

    let podsekce = Object.keys(aktualniNapoveda);
    if (podsekce.length > 1 && podsekce[0] == "&Nazev") {
        podsekce.shift();
    }

    podsekce.forEach(sekce => {
        if (sekce == "$Nazev" || sekce == cil) {
            let index = podsekce.indexOf(sekce);
            if (index !== -1) {
                podsekce.splice(index, 1);
            }
        }
    });

    if (podsekce.length != 0) {
        html += "<ul>";
        podsekce.forEach(sekce => {
            let aktualniAdresa = "";
            if (adresa != "") {
                adresa.forEach(segment => {
                    aktualniAdresa += segment + "-";
                });
                aktualniAdresa += [sekce];
            }
            else
                aktualniAdresa = [sekce];
            html += '<li><a href="/Napoveda/' + aktualniAdresa + '">' + aktualniNapoveda[sekce]["&Nazev"] + "</a></li>";
        });
        html += "</ul>";
    }

    return html;
}
    
let ROUTE = document.getElementById("ROUTE").innerHTML.split("-");

function load(sekce) {
    $.getJSON("../napoveda/napoveda.json",
        function(napoveda) {
            const prehravacObsah = document.getElementById("napoveda_prehravac_obsah");
            const prehravacNadpis = document.getElementById("napoveda_prehravac_nadpis");
            let html = "../napoveda/html/";
            let cesta = "";

            let aktualniNapoveda = napoveda;

            document.getElementById("napoveda_navigace_menu").innerHTML =
                document.getElementById("napoveda_navigace_menu").innerHTML + ziskatNavigaci(napoveda, true);

            try {
                sekce.forEach(rt => {
                    aktualniNapoveda = aktualniNapoveda[rt];
                    html += rt + "/";
                    cesta += aktualniNapoveda["&Nazev"];
                    if (rt != sekce[sekce.length - 1])
                        cesta += " > ";
                });

                prehravacNadpis.innerHTML = aktualniNapoveda["&Nazev"];
                fetch(html + "Index.html")
                    .then(response => response.text())
                    .then(text => prehravacObsah.innerHTML = text);
                document.getElementById("napoveda_navigace_menu_cesta").innerHTML = cesta;

                let podnavigace = ziskatNavigaci(aktualniNapoveda, false, document.getElementById("ROUTE").innerHTML);
                let podobne = ziskatSouvisejici(napoveda, sekce);
                if (podnavigace.trim().length > 0)
                    podnavigace = '<div id="napoveda_podsekce"><h2>Navazující</h2>' + podnavigace + '</div>';
                if (podobne.trim().length > 0)
                    podobne = '<div id="napoveda_souvisejici"><h2>Související</h2>' + podobne + '</div>';

                document.getElementById("navigace_podobnosti").innerHTML += podnavigace;
                document.getElementById("navigace_podobnosti").innerHTML += podobne;

                $("#napoveda_navigace_rozbalit").click(() => {
                    if (document.getElementById("napoveda_menu_seznam").className === "skryty") {
                        document.getElementById("napoveda_menu_seznam").className = "";
                    } else {
                        document.getElementById("napoveda_menu_seznam").className = "skryty";
                    }
                });
            }

            catch (chyba) {
                //console.log(chyba);
                prehravacNadpis.innerHTML = "Došlo k chybě!";
                prehravacObsah.innerHTML = "<article><section><i>Obsah této nápovědy není nyní přístupný!</i></section></article>";
            }
        }
    );
}

$("#napoveda_navigace_zpet").click(() => {
    if (ROUTE.length != 1) {
        document.getElementById("ROUTE").innerHTML = document.getElementById("ROUTE").innerHTML.replace(ROUTE[ROUTE.length - 1], "");
        document.getElementById("ROUTE").innerHTML =
            document.getElementById("ROUTE")
                .innerHTML.substring(0, document.getElementById("ROUTE").innerHTML.length - 1);

        ROUTE = document.getElementById("ROUTE").innerHTML.split("-");
        load(ROUTE);
    }
});

$('body').on('click',
    'img',
    (e) => {
        window.location.href = e.target.src;
    });

load(ROUTE);
