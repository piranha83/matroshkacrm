// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
$.fn.dataTable.render = $.fn.dataTable.render || {};
const root = '//localhost:5000/v1.0';
let error = (err) => console.error(err);

let credential = class {
    static async authentificate(login = 'root', password = '1') {
        var token = await $.post(`${root}/JwtAuthentification`, {
            login: login,
            password: password
        }).catch(error);

        $.ajaxSetup({
            beforeSend: (xhr, settings) => {
                //if (!(/^http:.*/.test(settings.url) || /^https:.*/.test(settings.url)))
                xhr.setRequestHeader('Authorization', `Bearer ${token}`);
            }
        });
    }
};

class Service {
    constructor(entity) {
        this.entity = entity;
    }

    async get(id = null) {
        return await $.get(id == null
            ? `${root}/${this.entity}`
            : `${root}/${this.entity}(${id})`)
            .catch(error);
    }

    async getColumns() {
        const columns = [];
        const xml = await $.get(`${root}/$metadata`).catch(error);
        $(xml)
            .find('EntityType')
            .filter((i, entityType) => $(entityType).attr('Name') == this.entity)
            .find('Property')
            .each((i, entityType) => columns.push({
                mData: $(entityType).attr('Name'),
                sType: function (type) {
                    switch (type) {
                        case 'Edm.Int32':
                        case 'Edm.Int16':
                            return 'numeric';
                        case 'Edm.DateTimeOffset':
                            return 'date';
                        case 'Edm.Decimal':
                            return 'currency';
                    }
                }($(entityType).attr('Type')),
            }));
        return columns;
    }

    getColumnDef(columns) {
        const columnDefs = [];
        $(columns).each((i, column) =>            
            columnDefs.push({
                targets: i,
                render: function (type) {
                    switch (type) {
                        /*case 'currency':
                            return $.fn.dataTable.render.ellipsis(20, false, true);
                        case 'numeric':
                            return $.fn.dataTable.render.number(',', '.', 0);
                        case 'date':
                            return $.fn.dataTable.render.moment('Do MMM YYYYY');*/
                    }
                }(column.sType)
            }));
        return columnDefs;
    }
}

let employee = new class Employee extends Service {
    constructor() {
        super('Employees')
    }

    async init() {
        var columns = await this.getColumns();
        columns = columns.filter((column) => {
            return column.mData != 'photo'
                && column.mData != 'photoPath'
                && column.mData != 'homePhone'
                && column.mData != 'notes';
        });
        console.log(columns);
        $('table').dataTable({
            'sAjaxSource': `${root}/${this.entity}`,
            'iODataVersion': 4,
            'aoColumns': columns,
            'fnServerData': fnServerOData, // required
            'bServerSide': true,  // optional
            'bUseODataViaJSONP': false,  // set to true if using cross-domain requests
            'columnDef': this.getColumnDef(columns)
        });
    }
}

// Write your JavaScript code.
$(document).ready(function () {
    credential.authentificate().then(async () => {
        await employee.init();
    }).catch(error);
});