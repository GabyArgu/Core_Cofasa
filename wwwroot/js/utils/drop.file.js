let dropArea;
let dragText;
let button;
let input;
let file;
// let removeFileButton;


function listenFileDrop() {
    dropArea = document.querySelector('.drag-file-container');
    dragText = dropArea.querySelector('h6');
    button = dropArea.querySelector('button#browseFiles');
    input = dropArea.querySelector('input[type="file"]');
    // removeFileButton = document.querySelector('button#removeFile');


    button.onclick = ()=> {
        input.click();
    }

    // removeFileButton.onclick = ()=> {
    //     removeFile();
    // }

    input.addEventListener('change', function(){
        file = this.files[0];
        dropArea.classList.add('active');
        viewfile();
    });

    dropArea.addEventListener('dragover', (event)=>{
        event.preventDefault();
        dropArea.classList.add('active');
        dragText.textContent = 'Suelta el archivo aquí';
    });


    dropArea.addEventListener('dragleave', ()=>{
        dropArea.classList.remove('active');
        dragText.textContent = 'Arrastra el archivo aquí';
    });

    dropArea.addEventListener('drop', (event)=>{
        event.preventDefault();

        file = event.dataTransfer.files[0];
        viewfile();
    });

    $('#uploadFileForm').on('submit', function(e) {
        e.preventDefault();
        let formData = new FormData(this);
        formData.append('file', file);
        uploadFile(formData);
    });

    $('#downloadCsvFormat').on('click', function(e) {
        e.preventDefault();
        window.open('/Repository/DownloadExcelFormat', '_blank');
    });
}

function removeFile(){
    dropArea.innerHTML = `
        <div class="icon"><i class="fas fa-cloud-upload-alt"></i></div>
        <header>Arrastra y suelta un archivo aquí o</header>
        <button id="browseFiles">Buscar archivos</button>
        <input type="file">
    `;
    dropArea.classList.remove('active');
    dragText.textContent = 'Arrastra el archivo aquí';
    // removeFileButton.style.display = 'none';
    listenFileDrop();
}

function viewfile(){
    let fileType = file.type;
    let validExtensions = [
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        'application/vnd.ms-excel',
        'application/vnd.ms-excel.sheet.macroEnabled.12',
        'application/vnd.oasis.opendocument.spreadsheet',
        'application/vnd.ms-excel.sheet.binary.macroEnabled.12',
        // 'text/csv',
    ];

    if(validExtensions.includes(fileType)){
        let fileReader = new FileReader();
        fileReader.onload = ()=> {
            let csvFileHtml = `
                <div class="icon"><i class="fas fa-file-excel-o"></i></div>
                <span>${file.name}</span>
                <span>${(file.size/1024).toFixed(2)} KB</span>
               <!-- <button id="removeFile" onclick="removeFile()">Eliminar</button>-->
            `;
            dropArea.innerHTML = csvFileHtml;
            //enable button
            $('#importFileButton').attr('disabled', false);
            // removeFileButton.style.display = '';
        }
        fileReader.readAsDataURL(file);
    }else{
        showToast(false, 'Este no es un archivo Excel', 'error');
        dropArea.classList.remove('active');
        dragText.textContent = 'Arrastra el archivo aquí';
    }
}

function uploadFile(formData) {
    isLoading('#importFileButton', true);

    $.ajax({
        url: '/Repository/ImportFromExcel',
        type: 'POST',
        processData: false,
        contentType: false,
        mimeType: 'multipart/form-data',
        data: formData,
        dataType: 'json',
        success: function(data) {
            isLoading('#importFileButton', false);
            showToast(data.success, data.message);

            if(data.success) {
                importFromFileDialog.modal('hide');
                reloadTable();
            }
        },
        error: function() {
            showToast(false, 'Ocurrió un error al procesar el archivo');
            isLoading('#importFileButton', false);
        }
    });
}