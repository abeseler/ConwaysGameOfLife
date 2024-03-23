using System.Text;

var rounds = 0;
var width = 160;
var height = 40;
var state = new int[width, height];
var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
var cts = new CancellationTokenSource();
var board = new StringBuilder($"Round: {rounds}\n");
var changes = new List<(int, int, int)>();
var random = new Random(420);

Console.Title = "Conway's Game of Life";
Console.CursorVisible = false;

for (var y = 0; y < height; y++)
{
    for (var x = 0; x < width; x++)
    {
        state[x, y] = random.Next(0, 2);
        board.Append(state[x, y] == 1 ? 'X' : ' ');
    }
    board.Append('\n');
}

while (await timer.WaitForNextTickAsync(cts.Token))
{
    rounds++;
    board.Clear();
    board.Append($"Round: {rounds}\n");
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            var value = state[x, y];
            var neighbors = 0;
            for (var dy = -1; dy <= 1; dy++)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    var nx = x + dx;
                    var ny = y + dy;

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        continue;

                    neighbors += state[nx, ny];
                }
            }
            if (neighbors < 2)
                value = 0;
            else if (neighbors > 3)
                value = 0;
            else if (neighbors == 3)
                value = 1;

            board.Append(value == 1 ? 'X' : ' ');

            if (state[x, y] != value)
                changes.Add((x, y, value));
        }
        board.Append('\n');
    }
    foreach (var (x, y, value) in changes)
    {
        state[x, y] = value;
    }
    Console.Clear();
    Console.Write(board.ToString());
}
