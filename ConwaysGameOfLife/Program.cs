const char ALIVE = 'X';
const char DEAD = ' ';

var seed = GetSeedFromUser();

Console.CursorVisible = false;
Console.Clear();
var rows = Console.WindowHeight - 1;
var columns = Console.WindowWidth - 1;
var random = new Random(seed);
var state = new char[rows][];
var buffer = new char[rows][];
for (var i = 0; i < rows; i++)
{
    state[i] = new char[columns];
    buffer[i] = new char[columns];
    for (var j = 0; j < columns; j++)
    {
        var value = random.Next(0, 2) == 1 ? ALIVE : DEAD;
        state[i][j] = value;
        Console.SetCursorPosition(j, i);
        Console.Write(value);
    }
}
var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
var cts = new CancellationTokenSource();

while (cts.IsCancellationRequested is false)
{
    PlayRound(state, buffer);

    await timer.WaitForNextTickAsync(cts.Token);
    Render(state, buffer);
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

static void PlayRound(char[][] state, char[][] buffer)
{
    for (var i = 0; i < state.Length; i++)
    {
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
}

static void Render(char[][] state, char[][] buffer)
{
    for (var i = 0; i < state.Length; i++)
    {
        for (var j = 0; j < state[i].Length; j++)
        {
            if (state[i][j] != buffer[i][j])
            {
                state[i][j] = buffer[i][j];
                Console.SetCursorPosition(j, i);
                Console.Write(state[i][j]);
            }
        }
    }
}
