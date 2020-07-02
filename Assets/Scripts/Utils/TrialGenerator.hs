{-# LANGUAGE LambdaCase #-}

import Data.List (intercalate, elemIndex)
import Data.Char (toUpper)
import Data.Maybe (fromJust)

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

instance (Writable a) => Writable [a] where
    write = intercalate "\n" . map write

perform :: (Prompt -> Challenge) -> Prompt -> TrialEntry
perform t s = Perform (t s)

type Main = IO ()

labelTrial :: Trial -> Int -> Trial
labelTrial trial n = Do (TrialNumber n) : trial

writeTrial :: Int -> Trial -> Main
writeTrial n trial = writeFile ("Assets/Trials/trial" ++ show n ++ ".txt") (write trial)

writeTrials :: [Main] -> Main
writeTrials [] = putStr "done."
writeTrials (first:rest) =
    do
        first
        writeTrials rest

main :: Main
main = writeTrials $ map writer $ zip trials [0..]
    where writer (t, n) = writeTrial n $ labelTrial t n


-- TRIALS

trials :: [Trial]
trials = 
    [ trialA
    , trialB 
    ] 

trialA :: Trial
trialA =
    [ Comment "arbitrarily made by logan"
    , Do (SetLayout TwoAxis)
    , perform Blind "the dog took a leap"
    , Do (SetLayout Raycast)
    , perform Perfect "and hit the ground softly"
    ]

trialB :: Trial
trialB =
    [ Comment "arbitrarily made by logan"
    , Do RandomizeLayoutOrder
    , perform Practice "abcde"
    , perform Blind "123456"
    ]