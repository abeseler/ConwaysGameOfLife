const char ALIVE = 'X';
const char DEAD = ' ';

var seed = GetSeedFromUser();

Console.CursorVisible = false;
var rows = Console.WindowHeight - 1;
var columns = Console.WindowWidth - 1;
var random = new Random(seed);
var state = new char[rows][];
for (var i = 0; i < rows; i++)
{
    state[i] = new char[columns];
    for (var j = 0; j < columns; j++)
    {
        state[i][j] = random.Next(0, 2) == 1 ? ALIVE : DEAD;
    }
}
var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(120));
var cts = new CancellationTokenSource();

while (await timer.WaitForNextTickAsync(cts.Token))
{
    Render(state);
    PlayRound(state);
}

static int GetSeedFromUser()
{
    Console.WriteLine("Enter a seed value:");    
    while (true)
    {
        var input = Console.ReadLine();
        if (int.TryParse(input, out var seed))
        {
            return seed;
        }
        Console.WriteLine("Invalid input. Please enter a valid number.");
    }
}

static void PlayRound(char[][] state)
{
    var buffer = new char[state.Length][];
    for (var i = 0; i < state.Length; i++)
    {
        buffer[i] = new char[state[i].Length];
        for (var j = 0; j < state[i].Length; j++)
        {
            var neighbors = 0;
            for (var y = i - 1; y <= i + 1; y++)
            {
                for (var x = j - 1; x <= j + 1; x++)
                {
                    if (x == j && y == i)
                        continue;

                    if (y < 0 || y >= state.Length || x < 0 || x >= state[i].Length)
                        continue;

                    if (state[y][x] == ALIVE)
                        neighbors++;
                }
            }

            buffer[i][j] = neighbors switch
            {
                2 => state[i][j],
                3 => ALIVE,
                _ => DEAD
            };
        }
    }
    for (var i = 0; i < state.Length; i++)
    {
        for (var j = 0; j < state[i].Length; j++)
        {
            state[i][j] = buffer[i][j];
        }
    }
}

static void Render(char[][] state)
{
    Console.Clear();
    for (var y = 0; y < state.Length; y++)
    {
        Console.WriteLine(state[y]);
    }
}
