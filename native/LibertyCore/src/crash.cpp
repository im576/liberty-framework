// Crash capture (ADR-0006): an unhandled exception in the game process writes a minidump plus a short text report
// naming the faulting module and the engine phase (which Liberty module was running), then the previous handler runs.
#include <windows.h>
#include <cstdio>
#include <cwchar>
#include "liberty_core.h"

namespace
{
    // Minimal MiniDumpWriteDump declaration (dbghelp.dll is loaded at install time, not linked).
    struct MinidumpExceptionInformation { DWORD thread_id; EXCEPTION_POINTERS* pointers; BOOL client_pointers; };
    typedef BOOL(WINAPI* MiniDumpWriteDumpFn)(HANDLE, DWORD, HANDLE, int, const MinidumpExceptionInformation*, const void*, const void*);
    const int MiniDumpNormal = 0x0, MiniDumpWithThreadInfo = 0x1000, MiniDumpWithIndirectlyReferencedMemory = 0x40;

    const int kPhases = 64;
    char g_phase_names[kPhases][48];
    volatile LONG g_phase = -1;
    wchar_t g_dir[MAX_PATH];
    MiniDumpWriteDumpFn g_write = nullptr;
    LPTOP_LEVEL_EXCEPTION_FILTER g_previous = nullptr;
    volatile LONG g_entered = 0;

    LONG WINAPI Filter(EXCEPTION_POINTERS* info)
    {
        if (InterlockedExchange(&g_entered, 1) == 0 && g_dir[0] != 0)
        {
            SYSTEMTIME t;
            GetLocalTime(&t);
            wchar_t base[MAX_PATH + 64];
            swprintf(base, MAX_PATH + 64, L"%ls\\crash-%04u%02u%02u-%02u%02u%02u", g_dir, t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond);
            wchar_t path[MAX_PATH + 80];
            swprintf(path, MAX_PATH + 80, L"%ls.dmp", base);
            HANDLE file = CreateFileW(path, GENERIC_WRITE, 0, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
            if (file != INVALID_HANDLE_VALUE && g_write != nullptr)
            {
                MinidumpExceptionInformation mei{ GetCurrentThreadId(), info, FALSE };
                g_write(GetCurrentProcess(), GetCurrentProcessId(), file, MiniDumpNormal | MiniDumpWithThreadInfo | MiniDumpWithIndirectlyReferencedMemory, &mei, nullptr, nullptr);
            }
            if (file != INVALID_HANDLE_VALUE) { CloseHandle(file); }

            void* address = info != nullptr && info->ExceptionRecord != nullptr ? info->ExceptionRecord->ExceptionAddress : nullptr;
            DWORD code = info != nullptr && info->ExceptionRecord != nullptr ? info->ExceptionRecord->ExceptionCode : 0;
            wchar_t module[MAX_PATH] = L"unknown";
            HMODULE owner = nullptr;
            if (address != nullptr && GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                reinterpret_cast<LPCWSTR>(address), &owner))
            {
                GetModuleFileNameW(owner, module, MAX_PATH);
            }
            LONG phase = g_phase;
            const char* phase_name = phase >= 0 && phase < kPhases ? g_phase_names[phase] : "none";
            swprintf(path, MAX_PATH + 80, L"%ls.txt", base);
            FILE* text = _wfopen(path, L"w");
            if (text != nullptr)
            {
                fwprintf(text, L"exception=0x%08lX address=0x%p module_base=0x%p module=%ls\nliberty_phase=%hs\n",
                    static_cast<unsigned long>(code), address, static_cast<void*>(owner), module, phase_name);
                fclose(text);
            }
        }
        return g_previous != nullptr ? g_previous(info) : EXCEPTION_CONTINUE_SEARCH;
    }
}

LC_API int32_t lc_install_crash_handler(const wchar_t* dir)
{
    if (dir == nullptr || wcslen(dir) >= MAX_PATH) { return 0; }
    wcsncpy(g_dir, dir, MAX_PATH - 1);
    CreateDirectoryW(g_dir, nullptr);
    HMODULE dbghelp = LoadLibraryW(L"dbghelp.dll");
    g_write = dbghelp != nullptr ? reinterpret_cast<MiniDumpWriteDumpFn>(reinterpret_cast<void*>(GetProcAddress(dbghelp, "MiniDumpWriteDump"))) : nullptr;
    static bool installed = false;
    if (!installed) { g_previous = SetUnhandledExceptionFilter(Filter); installed = true; }
    return g_write != nullptr ? 1 : 0;
}

LC_API void lc_register_phase(int32_t index, const char* name)
{
    if (index < 0 || index >= kPhases || name == nullptr) { return; }
    strncpy(g_phase_names[index], name, sizeof(g_phase_names[index]) - 1);
}

LC_API void lc_set_phase(int32_t index) { InterlockedExchange(&g_phase, index); }

LC_API int32_t lc_get_phase(void) { return g_phase; }

LC_API int32_t lc_write_dump(const char* reason)
{
    if (g_dir[0] == 0 || g_write == nullptr) { return 0; }
    SYSTEMTIME t;
    GetLocalTime(&t);
    wchar_t base[MAX_PATH + 64];
    swprintf(base, MAX_PATH + 64, L"%ls\\stall-%04u%02u%02u-%02u%02u%02u", g_dir, t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond);
    wchar_t path[MAX_PATH + 80];
    swprintf(path, MAX_PATH + 80, L"%ls.dmp", base);
    HANDLE file = CreateFileW(path, GENERIC_WRITE, 0, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (file == INVALID_HANDLE_VALUE) { return 0; }
    BOOL ok = g_write(GetCurrentProcess(), GetCurrentProcessId(), file, MiniDumpNormal | MiniDumpWithThreadInfo, nullptr, nullptr, nullptr);
    CloseHandle(file);
    LONG phase = g_phase;
    swprintf(path, MAX_PATH + 80, L"%ls.txt", base);
    FILE* text = _wfopen(path, L"w");
    if (text != nullptr)
    {
        fprintf(text, "reason=%s\nliberty_phase=%s\n", reason != nullptr ? reason : "", phase >= 0 && phase < kPhases ? g_phase_names[phase] : "none");
        fclose(text);
    }
    return ok ? 1 : 0;
}