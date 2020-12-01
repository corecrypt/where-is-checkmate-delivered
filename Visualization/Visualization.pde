import java.util.*;

final int BoardSize = 8;
final int ScreenSize = 1024;
final int SquareSize = ScreenSize / BoardSize;
final int HalfSquare = SquareSize / 2;
final color minCol = color(50, 100, 250);
final color maxCol = color(250, 100, 35);



void setup() {
  size(1124, 1024);

  visualizeData(loadData("white_checkmates.txt"));
  save("output_white.png");
  visualizeData(loadData("black_checkmates.txt"));
  save("output_black.png");
}

void drawScale() {
  noStroke();
  for (int y = 0; y < height; y++) {
    fill(lerpColor(minCol, maxCol, y / (float)height));
    rect(width - 100 + 1, height - y, 100, 1);
  }

  noFill();
  stroke(0);
  rect(width - 100, 0, 100 - 1, height);

  fill(0);
  textSize(32);
  text("Most", width - 90, 35);
  text("Least", width - 90, height - 20);
}

void visualizeData(ArrayList<Integer> data) {
  int maximum = Collections.max(data);
  int minimum = Collections.min(data);

  colorMode(HSB, 1, 100, 100);
  for (int x = 0; x < BoardSize; x++) {
    for (int y = 0; y < BoardSize; y++) {
      int screenX = x * SquareSize;
      int screenY = y * SquareSize;
      // transform 2d coords to 1d index
      int index = x * BoardSize + y;
      int count = data.get(index);
      color fillCol = lerpColor(minCol, maxCol, map(count, minimum, maximum, 0, 1));
      fill(fillCol);
      rect(screenX, screenY, SquareSize, SquareSize);
    }
  }
  
  drawScale();
}

ArrayList<Integer> loadData(String fileName) {
  ArrayList<Integer> data = new ArrayList<Integer>();
  BufferedReader reader = createReader(fileName);
  String line = null;

  try {
    while ((line = reader.readLine()) != null) {
      data.add(Integer.parseInt(line));
    }
    reader.close();
  } 
  catch (IOException e) {
    e.printStackTrace();
  }

  return data;
} 
