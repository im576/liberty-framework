// Read-only inspection using PseudoDisassembler: no program/database mutation.
// Run with analyzeHeadless -process GTAIV.exe -readOnly -noanalysis.
import ghidra.app.script.GhidraScript;
import ghidra.app.util.PseudoDisassembler;
import ghidra.app.util.PseudoInstruction;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.Function;
import ghidra.program.model.symbol.ReferenceIterator;

public class frontend_t055_ghidra extends GhidraScript {
    public void run() throws Exception {
        println("T055_READ_ONLY program="+currentProgram.getName()+" md5="+currentProgram.getExecutableMD5()
                +" sha256="+currentProgram.getExecutableSHA256()+" base="+currentProgram.getImageBase()
                +" language="+currentProgram.getLanguageID()+" functions="+currentProgram.getFunctionManager().getFunctionCount());
        PseudoDisassembler dis = new PseudoDisassembler(currentProgram);
        for (String value : getScriptArgs()) {
            Address start = toAddr(value);
            Function f = currentProgram.getFunctionManager().getFunctionContaining(start);
            println("ANCHOR "+start+" function="+(f==null?"UNKNOWN":f.getName()+" entry="+f.getEntryPoint()));
            ReferenceIterator refs = currentProgram.getReferenceManager().getReferencesTo(start);
            int count = 0;
            while (refs.hasNext() && count++ < 30) println("DB_REF "+refs.next());
            Address at = start;
            for (int n=0; n<180 && !monitor.isCancelled(); n++) {
                PseudoInstruction ins = dis.disassemble(at);
                if (ins == null) { println("DECODE_FAILED "+at); break; }
                println(at+" "+ins.toString());
                at = at.add(ins.getLength());
                if (ins.getFlowType().isTerminal()) break;
            }
        }
    }
}
