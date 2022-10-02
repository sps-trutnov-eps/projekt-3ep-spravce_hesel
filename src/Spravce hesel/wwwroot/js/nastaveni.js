let zmena_jmena = document.getElementById("zmena_jmena");
let zmena_hesla = document.getElementById("zmena_hesla");
let odstraneni = document.getElementById("odstraneni");
let informace = document.getElementById("informace");

function zobrazit_zmena_jmena() {
    zmena_jmena.className = "";
    zmena_hesla.className = "skryty";
    odstraneni.className = "skryty";
    informace.className = "skryty";
};

function zobrazit_zmena_hesla() {
    zmena_hesla.className = "";
    zmena_jmena.className = "skryty";
    odstraneni.className = "skryty";
    informace.className = "skryty";
};

function zobrazit_odstraneni() {
    odstraneni.className = "";
    zmena_jmena.className = "skryty";
    zmena_hesla.className = "skryty";
    informace.className = "skryty";
};