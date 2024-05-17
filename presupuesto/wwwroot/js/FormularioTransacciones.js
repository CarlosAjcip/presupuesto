function InicializarFormularioTransacciones(urlObtenerCategorias) {

    $(function () {
        $("#id_TiposOp").change(async function () {
            const valorSeleccionado = $(this).val();

            const respuesta = await fetch(urlObtenerCategorias, {
                method: 'POST',
                body: valorSeleccionado,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const json = await respuesta.json();
            console.log(json);
            const opciones = json.map(categoria => `<option value = ${categoria.value}>${categoria.text}</option>`);
            $("#id_categorias").html(opciones);
            console.log(opciones);
        })
    })
}

