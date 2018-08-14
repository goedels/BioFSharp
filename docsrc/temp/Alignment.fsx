(*** hide ***)

#I "../../bin/BioFSharp/net45"
#r "BioFSharp.dll"
(**

<table class="HeadAPI">
<td class="Head"><h1>Pairwise Alignment</h1></td>
<td class="API">
    <a id="APILink" href="https://csbiology.github.io/BioFSharp/reference/biofsharp-algorithm-pairwisealignment.html" >&#128194;View module documentation</a>
</td>
</table>

In this short tutorial, the usage of the Pairwise alignment-implementation is demonstrated. 
For global alignments, the <b>NeedlemanWunsch</b>-algorithm is used. For local alignments, the <b>SmithWaterman</b>-algorithm is used.
For both implementations, the gapvalues are evaluated using the <b>affine</b> gapscoremodel. 
*)
open BioFSharp
open BioFSharp.Algorithm
open PairwiseAlignment

(**
<div class="box">
## Aligning aminoacid- and nucleotide-sequences


For defining the scores of matching and missmatching characters, the <b>scoring</b>-function is defined. In the case of Aminoacid- or Nucleotide-sequence-alignments, the integrated substitution-matrices can be used.
*)
//System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("") 
//For aminoacids
let aaScoring = ScoringMatrix.getScoringMatrixAminoAcid ScoringMatrix.ScoringMatrixAminoAcid.BLOSUM62

//For nucleotides
let nucScoring = ScoringMatrix.getScoringMatrixNucleotide  ScoringMatrix.ScoringMatrixNucleotide.EDNA

(**

In order to align two sequences, not only the values for substitutions, but also the <b>costs</b> for gaps are needed. In this implementation, an affine gap-penalty is realized. An affine gap penalty weights continuations of gaps different than the opening of a gap. 
The scoring function (in this case a substitution matrix) and the two gap penalty values are combined into the <b>Costs</b>-type.

*)

//For aminoacids
let aaCosts = {
    Open = -5
    Continuation = -2
    Similarity = aaScoring 
    }

//For nucleotides
let nucCosts = {
    Open = -2
    Continuation = -1
    Similarity = nucScoring 
    }

(**

The alignment functions use <b>Arrays</b> as input. The Elements can be of any type, but require type equality. Also they need to have type equality with the input of the scoring function.
Both the global and local alignment algorithms take the same parameters (costs,firstSequence,secondSequence) and return the same format.

*)

//For aminoacids
let aaSeq1 = "ACDM" |> BioArray.ofAminoAcidSymbolString
let aaSeq2 = "MAACEDM" |> BioArray.ofAminoAcidSymbolString

let globalAAAlignment = NeedlemanWunsch.runAminoAcidSymbol aaCosts aaSeq1 aaSeq2
let localAAAlignment = SmithWaterman.runAminoAcidSymbol aaCosts aaSeq1 aaSeq2

(** This returns:  
_The field MetaData carries the score of the alignment_
<pre>

val globalAAAlignment :
  Alignment.Alignment<AminoAcidSymbols.AminoAcidSymbol list,Score> =
  {MetaData = 12;
   AlignedSequences = [[-; -; A; C; -; D; M]; [M; A; A; C; E; D; M]];}

val localAAAlignment :
  Alignment.Alignment<AminoAcidSymbols.AminoAcidSymbol list,Score> =
  {MetaData = 19;
   AlignedSequences = [[A; C; -; D; M]; [A; C; E; D; M]];}
</pre>
*)

//For nucleotides
let nucSeq1 = "ATGA" |> BioArray.ofNucleotideString
let nucSeq2 = "BATVAWG" |> BioArray.ofNucleotideString

let globalNucAlignment = NeedlemanWunsch.runNucleotide nucCosts nucSeq1 nucSeq2
let localNucAlignment = SmithWaterman.runNucleotide nucCosts nucSeq1 nucSeq2

(**
</div>*)

(**
<div class="box">
## Aligning anything else


This implementation was aimed to be as generic as possible. To achieve this, the scoring function can be designed at will, the only constraints being the need for two input variables and the type equality.  
Also besides the alignment functions which only take BioItems as input and represent the gaps by the appropriate gaps of type BioItem. There is also a generic alignment function `runGeneric` which returns `lists of options`, where None represents gaps. Therefore any input type can be used, given it matches the cost function.  

For example, one could use a simple "if else" equality function to match nucleotides
*)

let scoring a b = 
    if a = b then 2
    else -2

let costs = {
    Open = -2
    Continuation = -1
    Similarity = scoring
    }


let globalAlignmentNuc = NeedlemanWunsch.runGeneric costs nucSeq1 nucSeq2
let localAlignmentNuc = SmithWaterman.runGeneric costs nucSeq1 nucSeq2


(** This returns:  
<pre>

val globalAlignmentNuc :
  Alignment.Alignment<Nucleotides.Nucleotide option list,Score> =
  {MetaData = -1;
   AlignedSequences =
    [[None; Some A; Some T; Some G; Some A; None; None];
     [Some B; Some A; Some T; Some V; Some A; Some W; Some G]];}

val localAlignmentNuc :
  Alignment.Alignment<Nucleotides.Nucleotide option list,Score> =
  {MetaData = 1;
   AlignedSequences =
    [[Some A; Some T; Some G; Some A]; [Some A; Some T; Some V; Some A]];}
</pre>

or also Integers:
*)

let intSeq1 = [|1;2;3;4;5|]
let intSeq2 = [|1;1;2;4;6;7;|]

let globalAlignmentInt = NeedlemanWunsch.runGeneric costs intSeq1 intSeq2
let localAlignmentInt = SmithWaterman.runGeneric costs intSeq1 intSeq2

(**
</div>*)
