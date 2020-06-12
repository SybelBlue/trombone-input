{-# LANGUAGE LambdaCase #-}

import Text.Printf
import Data.List

type Layout = (Bool, [LayoutKey])

data LayoutKey = Simple Char Size | Binned [ LayoutKey ] | Alt Char Size (Maybe Char) deriving Show

data Size = Small | Medium | Big deriving Show

sizeToBarWidth :: Size -> Int
sizeToBarWidth =
    \case
      Small -> 2
      Medium -> 3
      Big -> 4

qwerty :: Layout
qwerty = (False, map tripletIntoAmbiguous $ [ "QAZ", "WSX", "EDC", "RFV", "TGB"] ++ map reverse ["YHU", "IJN", "OKM", "PL." ])

tripletIntoAmbiguous :: String -> LayoutKey
tripletIntoAmbiguous [ a, b, c ] = Binned [ Simple a Small, Simple b Medium, Simple c Small ]
tripletIntoAmbiguous _ = error "unexpected input"

layoutSize :: [LayoutKey] -> Int
layoutSize = sum . map itemSize

itemSize :: LayoutKey -> Int
itemSize (Simple _ s) = sizeToBarWidth s
itemSize (Binned xs) = layoutSize xs
itemSize _ = sizeToBarWidth Small

abcde :: Layout
abcde = (False, map makeItem ['A'..'Z'])
  where makeItem x = Simple x $ if commonLetter x then Medium else Small

binnedAbcde :: Layout
binnedAbcde = (True, map Binned . splitEvery 4 . map makeItem $ zip base alt)
  where 
    base = (map (\c -> (c, Small)) ['A'..'Z']) ++ [('.', Medium), (' ', Medium)]
    makeItem ((c, s), a) = Alt c s a
    alt = map Just . concat . transpose $
          [ [ '1', '2', '3', '/', '@',  '-', ';' ]
          , [ '4', '5', '6', '%', '\'', '&', ':' ]
          , [ '7', '8', '9', '#', '\"', '?', ',' ]
          , [ '*', '+', '.', '(', ')', '!', '\b' ] 
          ]

charString :: Char -> String
charString = \case
  '\b' -> "\\b"
  '\'' -> "\\'"
  '\"' -> "\\\""
  x -> [x]

-- https://stackoverflow.com/questions/8680888/subdividing-a-list-in-haskell
splitEvery :: Int -> [a] -> [[a]]
splitEvery _ [] = []
splitEvery n list = first : (splitEvery n rest)
  where
    (first,rest) = splitAt n list

commonLetter :: Char -> Bool
commonLetter x = elem x "ERIOTAN"

makeInitMethod :: Layout -> String
makeInitMethod (stylusMode, keys) = "\t// Auto-generated \n\tprotected override LayoutKey[] FillKeys() {\n\t\t"
  ++ "return new LayoutKey[] {\n"
  ++ (intercalate "\n" $ concat $ map (map deformat . makeConstructorLineFor stylusMode) keys)
  ++ "\n\t};\n}"


type Formatted = (Int, String, Bool)

deformat :: Formatted -> String
deformat (n, s, b) = (take n $ cycle "\t") ++ s ++ if b then "," else ""

constructorName :: Bool -> LayoutKey -> String
constructorName stylusMode (Binned _) = if stylusMode then "StylusBinnedKey" else "AmbiguousKey"
constructorName stylusMode _ = if stylusMode then "StylusKey" else "SimpleKey"

makeConstructorLineFor :: Bool -> LayoutKey -> [Formatted]
makeConstructorLineFor stylusMode (Simple c s) = [(2, printf "new %s('%s', %d)" name (charString c) (sizeToBarWidth s), True)]
  where name = constructorName stylusMode $ Simple c s
makeConstructorLineFor stylusMode (Alt c s x) = [(2, printf "new %s('%s', %d%s)" name (charString c) (sizeToBarWidth s) lastArg, True)]
  where
    name = constructorName stylusMode $ Alt c s x
    lastArg = 
      case x of 
          Nothing -> ""
          Just a -> printf ", '%s'" $ charString a
makeConstructorLineFor stylusMode (Binned items) = (2, "new " ++ name ++ "(true", True) : inner ++ [(2, ")", True)]
  where 
    name = constructorName stylusMode $ Binned items
    inner =
      case uncons . reverse . map (\(n, s, b) -> (n + 1, s, b)) . concat $ map (makeConstructorLineFor stylusMode) items of
        Just ((n, s, _), others) ->
          reverse $ (n, s, False) : others
        Nothing ->
          []
  

main = putStr $ makeInitMethod binnedAbcde