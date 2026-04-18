using System.Diagnostics;
using System.Text;
using System.Text.Json;

var api = new ApiService("http://localhost/api");

api.RequestCompleted += LogToConsole;
api.RequestCompleted += LogToFile;

Console.WriteLine("=== Операции 1-3: консоль + файл ===\n");

Console.WriteLine("[1] GET /doctors");
var doctors = await api.Get("/doctors");
Console.WriteLine($"    {api.FormatResponse(doctors)}\n");

Console.WriteLine("[2] GET /patients");
var patients = await api.Get("/patients");
Console.WriteLine($"    {api.FormatResponse(patients)}\n");

Console.WriteLine("[3] POST /appointments");
var appt = await api.Post("/appointments", new {
    patientId = 1,
    doctorId = 1,
    dateTime = "2026-04-20T10:00:00Z",
    reason = "Консультация",
    status = "pending"
});
Console.WriteLine($"    {api.FormatResponse(appt)}\n");

Console.WriteLine("--- Отписываем LogToFile (-=) ---\n");
api.RequestCompleted -= LogToFile;

Console.WriteLine("[4] GET /appointments");
var appts = await api.Get("/appointments");
Console.WriteLine($"    {api.FormatResponse(appts)}\n");

Console.WriteLine("[5] GET /prescriptions");
var presc = await api.Get("/prescriptions");
Console.WriteLine($"    {api.FormatResponse(presc)}\n");

Console.WriteLine("Готово! Файл requests.log содержит только первые 3 запроса.");

static void LogToConsole(string ep, int code, long ms) =>
    Console.WriteLine($"  [OK] {ep,-25} -> {code} ({ms}ms)");

static void LogToFile(string ep, int code, long ms) =>
    File.AppendAllText("requests.log", $"{DateTime.Now:HH:mm:ss} | {ep} | {code} | {ms}ms\n");

delegate void OnRequestCompleted(string endpoint, int statusCode, long elapsedMs);

class ApiService
{
    private readonly HttpClient _http = new();
    private readonly string _base;
    public OnRequestCompleted? RequestCompleted;
    public Action<string> OnError = msg => Console.WriteLine($"[ОШИБКА] {msg}");
    public Func<string, string> FormatResponse = raw =>
        raw.Length > 100 ? raw[..100] + "..." : raw;

    public ApiService(string baseUrl) { _base = baseUrl; }

    public async Task<string> Get(string path)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var resp = await _http.GetAsync(_base + path);
            sw.Stop();
            RequestCompleted?.Invoke(path, (int)resp.StatusCode, sw.ElapsedMilliseconds);
            return await resp.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { OnError(ex.Message); return ""; }
    }

    public async Task<string> Post(string path, object body)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(_base + path, content);
            sw.Stop();
            RequestCompleted?.Invoke(path, (int)resp.StatusCode, sw.ElapsedMilliseconds);
            return await resp.Content.ReadAsStringAsync();
        }
        catch (Exception ex) { OnError(ex.Message); return ""; }
    }

    public async Task<string> Delete(string path)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var resp = await _http.DeleteAsync(_base + path);
            sw.Stop();
            RequestCompleted?.Invoke(path, (int)resp.StatusCode, sw.ElapsedMilliseconds);
            return resp.StatusCode.ToString();
        }
        catch (Exception ex) { OnError(ex.Message); return ""; }
    }
}
