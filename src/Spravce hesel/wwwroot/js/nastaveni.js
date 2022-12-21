let zmena_jmena = document.getElementById("zmena_jmena");
let zmena_hesla = document.getElementById("zmena_hesla");
let odstraneni = document.getElementById("odstraneni");
let informace = document.getElementById("informace");

function zobrazit_zmena_jmena() {
    if (zmena_jmena.className == "skryty") {
        zmena_jmena.className = "";
        zmena_hesla.className = "skryty";
        odstraneni.className = "skryty";
        informace.className = "skryty";
    }
    else
        zobrazit_informace();
};

function zobrazit_zmena_hesla() {
    if (zmena_hesla.className == "skryty") {
        zmena_hesla.className = "";
        zmena_jmena.className = "skryty";
        odstraneni.className = "skryty";
        informace.className = "skryty";
    }
    else
        zobrazit_informace();
};

function zobrazit_odstraneni() {
    if (odstraneni.className == "skryty") {
        odstraneni.className = "";
        zmena_jmena.className = "skryty";
        zmena_hesla.className = "skryty";
        informace.className = "skryty";
    }
    else
        zobrazit_informace();
};

function zobrazit_informace() {
    informace.className = "";
    odstraneni.className = "skryty";
    zmena_jmena.className = "skryty";
    zmena_hesla.className = "skryty";
}

$(".zmena_jmena").click(zobrazit_zmena_jmena);

$(".zmena_hesla").click(zobrazit_zmena_hesla);

$(".odstraneni").click(zobrazit_odstraneni);