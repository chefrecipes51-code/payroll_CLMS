function renderPagination(totalPages, currentPage) {
    let html = "";

    if (totalPages <= 0) {
        $('#pagination-controls').html('');
        return;
    }

    const maxShow = 7;

    if (currentPage > 1) {
        html += `<button class="page-btn btn btn-sm btn-outline-primary" data-page="${currentPage - 1}">Prev</button>`;
    }

    if (totalPages <= maxShow) {
        for (let i = 1; i <= totalPages; i++) {
            html += `<button class="page-btn btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'}" data-page="${i}">${i}</button>`;
        }
    } else {
        html += `<button class="page-btn btn btn-sm ${currentPage === 1 ? 'btn-primary' : 'btn-outline-primary'}" data-page="1">1</button>`;
        if (currentPage > 4) html += `<span class="mx-1">…</span>`;

        const start = Math.max(2, currentPage - 1);
        const end = Math.min(totalPages - 1, currentPage + 1);
        for (let i = start; i <= end; i++) {
            html += `<button class="page-btn btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-outline-primary'}" data-page="${i}">${i}</button>`;
        }

        if (currentPage < totalPages - 3) html += `<span class="mx-1">…</span>`;
        html += `<button class="page-btn btn btn-sm btn-outline-primary" data-page="${totalPages}">${totalPages}</button>`;
    }

    if (currentPage < totalPages) {
        html += `<button class="page-btn btn btn-sm btn-outline-primary" data-page="${currentPage + 1}">Next</button>`;
    }

    $('#pagination-controls').html(html);
}

function makeDataTableNew(selector) {
    $(selector).DataTable({
        paging: false,
        info: false,
        lengthChange: false,
        searching: true,
        ordering: true,
        dom: '<"d-flex justify-content-between align-items-center px-3 pt-2"f>t',
        language: {
            search: ""
        },
        scrollY: true,
        fixedHeader: true,
        columnDefs: [{ orderable: false, targets: 0 }]
    });

    // Inject custom search box with icon
    setTimeout(() => {
        const searchBox = $(`${selector}_filter input`);
        if (searchBox.length) {
            $('#customSearch').html(`
                <div class="position-relative">
                    <span class="position-absolute top-50 start-0 translate-middle-y ps-2">
                        <img src="/assets/img/icons/search.svg" width="16" height="16" />
                    </span>
                    <input type="search" class="form-control ps-5 rounded" placeholder="Search..." aria-label="Search" value="${searchBox.val()}">
                </div>
            `);

            // Attach event to new input
            $('#customSearch input').on('keyup', function () {
                $(selector).DataTable().search(this.value).draw();
            });
        }
    }, 300);
}
