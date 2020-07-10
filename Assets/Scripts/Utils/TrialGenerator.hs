{-# LANGUAGE LambdaCase #-}

import Data.List (intercalate)
import Data.Char (toUpper)
import Data.Tuple (fst)

-- | text prompts in the main scene
type Prompt = String

-- all of the trombone-input layouts
data Layout = Linear | StylusBinned | TwoAxis | Raycast deriving (Eq, Show)

-- | commands that are entered into the file for Unity to perform
data Command = RandomizeLayoutOrder | NextLayout | TrialNumber Int | SetLayout Layout deriving Show

{- |
    Challenges are individual items in the Trial file that Unity will read
    - Blind is the challenge where users can not backspace or view what they have typed
    - Perfect is the challenge where users must type the full prompt exactly
    - Practice is an untimed and unscored challenge that users may skip
-}
data Challenge = Blind Prompt | Perfect Prompt | Practice Prompt deriving Show

-- | challenges and commands that are written into trial files
data TrialEntry = Do Command | Perform Challenge | Comment String deriving Show

type Trial = [TrialEntry]


commandPrefixChar :: Char
commandPrefixChar = '!'
commentPrefixChar :: Char
commentPrefixChar = '#'
challengeSeperatorChar :: Char
challengeSeperatorChar = ':'

scrubPrompt :: Prompt -> Prompt
scrubPrompt p = map mapper p
    where
        mapper c
            | c == '\"' = error $ "prompt \'" ++ p ++ "\' cannot contain yaml delimiter \""
            | otherwise = toUpper c


layoutNumber :: Layout -> Int
layoutNumber = \case
    Linear -> 0
    StylusBinned -> 1
    TwoAxis -> 2
    Raycast -> 3

-- a class for writing to files
class Writable a where
    write :: a -> String

instance Writable Layout where
    write = show . layoutNumber

instance Writable Command where
    write RandomizeLayoutOrder = "randomize-layouts"
    write NextLayout = "next-layout"
    write (TrialNumber n) = "trial-number " ++ show n
    write (SetLayout l) = "set-layout " ++ write l

instance Writable Challenge where
    write (Blind p) = "blind" ++ [challengeSeperatorChar] ++ scrubPrompt p
    write (Perfect p) = "perfect" ++ [challengeSeperatorChar] ++ scrubPrompt p
    write (Practice p) = "practice" ++ [challengeSeperatorChar] ++ scrubPrompt p

instance Writable TrialEntry where
    write (Do c) = commandPrefixChar : write c
    write (Perform c) = write c
    write (Comment c) = commentPrefixChar : c

instance Writable a => Writable [a] where
    write = intercalate "\n" . map write

perform :: (Prompt -> Challenge) -> Prompt -> TrialEntry
perform t s = Perform (t s)

type Main = IO ()

labelTrial :: Trial -> Int -> Trial
labelTrial trial n = Do (TrialNumber n) : trial

writeTrial :: Int -> Trial -> Main
writeTrial n trial = writeFile ("Assets/StreamingAssets/Trials/Dummy" ++ show n ++ ".txt") (write trial)

doAll :: [Main] -> Main
doAll [] = putStr "done."
doAll (first:rest) =
    do
        first
        doAll rest

writeTemplateTrials :: Main
writeTemplateTrials =
    do
        content <- readFile "Assets/Scripts/Utils/MacKenzie2.txt"
        let strs = lines content 
        let templates = templateTrials strs
        doAll $ map writeTTrial $ zip [0..] $ templates 6
    where
        writeTTrial :: (Int, (Layout, Trial)) -> Main
        writeTTrial (n, (l, t)) = writeFile ("Assets/StreamingAssets/Trials/" ++ show n ++ "-" ++ show l ++ ".txt") $ write $ labelTrial ((Do (SetLayout l)):t) n

writeDummies :: Main
writeDummies = doAll $ map writer $ zip dummies [0..]
    where writer (t, n) = writeTrial n $ labelTrial t n

main :: Main
main =  writeTemplateTrials


-- TRIALS

dummies :: [Trial]
dummies = 
    [ dummyA
    , dummyB 
    ] 

dummyA :: Trial
dummyA =
    [ Comment "arbitrarily made by logan"
    , Do (SetLayout TwoAxis)
    , perform Blind "the dog took a leap"
    , Do (SetLayout Raycast)
    , perform Perfect "and hit the ground softly"
    ]

dummyB :: Trial
dummyB =
    [ Comment "arbitrarily made by logan"
    , Do RandomizeLayoutOrder
    , perform Practice "abcde"
    , perform Blind "123456"
    ]

template :: [String] -> (Trial, [String])
template strs = (challenges, remainder)
    where 
        n = 5
        remainder = drop (length ops) strs
        ops = map perform $ [Practice, Practice] ++ replicate n Blind ++ replicate n Perfect
        challenges = map (\(a, b) -> a b) $ zip ops $ take (length ops) strs


templateTrials :: [String] -> Int -> [(Layout, Trial)]
templateTrials strs r = fst $ foldl folder ([], strs) $ foldl (++) [] $ map (replicate r) [Linear, StylusBinned, TwoAxis, Raycast]
    where folder (trials, start) layout = let (t, end) = template start in ((layout, t):trials, end)
