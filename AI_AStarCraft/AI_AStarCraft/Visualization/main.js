/* exported app */

let app = {
    el: '#app',
    data: {
        hist: [],
        step: -1,
        table: [],
        colors: {
            190: 'rgb(255, 0, 0)',
            191: 'rgb(255, 255, 0)',
            192: 'rgb(0, 255, 255)',
            193: 'rgb(255, 0, 255)',
            1190: 'rgba(255, 0, 0, 0.5)',
            1191: 'rgba(255, 255, 0, 0.5)',
            1192: 'rgba(0, 255, 255, 0.5)',
            1193: 'rgba(255, 0, 255, 0.5)'
        },
    },
    methods: {
        getCell: v => v,
        getStyle: function (x, y) {
            if (this.table[x][y] === '#')
                return {
                    backgroundColor: '#090808',
                    border: '1px solid #1A1A1A'
                };
            if (/\d+/.test(this.table[x][y]))
                return {
                    backgroundColor: this.colors[this.table[x][y]],
                    border: '1px solid #1A1A1A'
                };
            return {
                backgroundColor: '#8c8c8c',
                border: '1px solid #424242'
            };
        },
        start: function () {
            this.table = start.Map.split('\n').map(line => line.split(''));
            let path = JSON.parse(data).data;
            let robots = start.Robots;
            for (let i = 0; i < robots.length; i++) {
                let r = robots[i].split(' ')
                this.table[r[1]][r[0]] = (190 + i).toString()
            }
            this.hist = iter(path.history);
            setTimeout(this.loop, 500, this.hist.next());
        },
        loop: function (elem) {
            console.log(this.hist)
            let e = elem.value
            this.table = start.Map.split('\n').map(line => line.split(''));
            this.step = e.i
            for (let r of e.robots) {
                if (this.table[r.y][r.x] === '.')
                    this.table[r.y][r.x] = (r.id).toString()
                else
                    this.table[r.y][r.x] = (r.id + 1000).toString()
            }
            if (!elem.done)
                setTimeout(this.loop, 500, this.hist.next())
        }
    },
    mounted: function () {
        this.start();
    }
};

function iter(array){
    let nextIndex = 0;
    return {
        next: function(){
            return nextIndex < array.length ?
                {value: array[nextIndex++], done: false} :
                {done: true};
        }
    }
}
